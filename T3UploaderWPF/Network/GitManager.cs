using LibGit2Sharp;
using LibGit2Sharp.Handlers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using T3UploaderWPF.Network.Data;
using T3UploaderWPF.Settings;
using T3UploaderWPF.UI.Data;

namespace T3UploaderWPF.Network
{
    public class GitManager
    {
        private static readonly Lazy<GitManager> LazyGitManager = new(() => new GitManager());
        private UsernamePasswordCredentials _usernamePasswordCredentials;

        public string RemoteRepository { get; private set; }
        public string LocalRepository { get; set; } = string.Empty;
        public Signature? Author { get; private set; }

        public static GitManager Instance { get { return LazyGitManager.Value; } }
        private GitManager()
        { }

        public ErrorHandler Initialize(SettingsGit settings)
        {

            var sb = new StringBuilder();
            if (settings.Remote != null)
            {
                RemoteRepository = settings.Remote;
            }
            else
            {
                sb.AppendLine("Missing remote");
            }
            if (settings.Author != null)
            {
                Author = new Signature(settings.Author.Username, settings.Author.Email, DateTime.UtcNow);
            }
            else
            {
                sb.AppendLine("Missing author");
            }

            if (settings.Account != null)
            {
                _usernamePasswordCredentials = new UsernamePasswordCredentials() { Username = settings.Account.Username, Password = settings.Account.Token };
            }
            else
            {
                sb.AppendLine("Missing Account info");
            }

            if (sb.Length > 0)
            {
                return new ErrorHandler()
                {
                    Success = false,
                    ErrorMessage = sb.ToString()
                };
            }
            return new ErrorHandler() { Success = true };
        }

        public void UpdateBraches(string branchName)
        {
            using var repo = new Repository(LocalRepository);
            Branch currentBranch;
            if (!branchName.Equals("master"))
            {
                var branch = repo.Branches[branchName];
                if (branch == null)
                    repo.CreateBranch(branchName);
                currentBranch = Commands.Checkout(repo, branchName);
            }
            else
            {
                currentBranch = repo.Branches[branchName];
            }

            var options = new PullOptions
            {
                FetchOptions = new FetchOptions
                {
                    CredentialsProvider = new CredentialsHandler(
                (url, usernameFromUrl, types) => _usernamePasswordCredentials)
                }
            };
            var logMessage = string.Empty;
            var remote = repo.Network.Remotes["origin"];
            var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
            Commands.Fetch(repo, remote.Name, refSpecs, options.FetchOptions, logMessage);

            var behindBy = currentBranch.TrackingDetails.BehindBy ?? 0;
            if (!string.IsNullOrEmpty(logMessage) || behindBy > 0)
            {
                Commands.Pull(repo, Author, options);
            }
            // Pull

            if (!branchName.Equals("master"))
                Commands.Checkout(repo, "master");
        }

        public GitCommitStatus LastCommitStatus()
        {
            using var repo = new Repository(LocalRepository);
            var status = repo.RetrieveStatus();
            var branch = repo.Branches["master"];
            var tracking = branch.TrackingDetails;

            var dirty = tracking.AheadBy > 0;
            return new GitCommitStatus { Dirty = dirty, Message = branch.Commits.First().Message };
        }

        public string? Clone(string workspaceDirectory, string root)
        {
            var co = new CloneOptions
            {
                CredentialsProvider = (_url, _user, _cred) => _usernamePasswordCredentials
            };

            return Repository.Clone(RemoteRepository, Path.Combine(workspaceDirectory, "."), co);
        }

        public void InitializeLocalRepository(string choosedFolder)
        {
            LocalRepository = choosedFolder;
            if (!Repository.IsValid(choosedFolder))
            {
                Repository.Init(choosedFolder);
                using var repo = new Repository(choosedFolder);
                repo.Network.Remotes.Add("origin", RemoteRepository);
            }
        }

        public ErrorHandler PushToRemote(string commitMessage, string branchName)
        {
            using var repo = new Repository(LocalRepository);

            var masterPush = PushToMaster(repo, commitMessage);

            if (!branchName.Equals("master"))
            {
                var masterBranch = repo.Branches["master"];
                var otherBranch = InitializeBranch(repo, branchName);
                _ = Commands.Checkout(repo, otherBranch);
                repo.Merge(masterBranch, Author);
                var otherPush = PushTo(repo, otherBranch);
                Commands.Checkout(repo, masterBranch);

                return otherPush;
            }
            else
            {
                return masterPush;
            }

        }

        private Branch InitializeBranch(Repository repo, string branchName)
        {

            var branch = repo.Branches[branchName];
            if (branch == null)
            {
                branch = repo.CreateBranch(branchName);
            }
            repo.Branches.Update(branch,
                    b => b.Remote = repo.Network.Remotes["origin"].Name,
                    b => b.UpstreamBranch = branchName);
            return branch;

        }

        private ErrorHandler PushToMaster(Repository repo, string message)
        {
            var options = new PushOptions
            {
                CredentialsProvider = (string _url, string _user, SupportedCredentialTypes _cred) => _usernamePasswordCredentials
            };
            var result = new ErrorHandler();
            try
            {
                // check commits append
                var status = repo.RetrieveStatus();
                if (repo.Diff.Compare<TreeChanges>().Count > 0 || status.Untracked.Any())
                {
                    Commands.Stage(repo, "*");
                    var commit = repo.Commit(message, Author, Author);
                }
                var branch = repo.Branches["master"];

                //var remote = repo.Network.Remotes["origins"];
                //repo.Network.Push(remote, @$"refs/heads/master", options);
                repo.Network.Push(branch, options);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = "Failed to push at master";
                result.Exception = ex;
                result.ExceptionMessage = ex.Message;
            }
            return result;
        }

        private ErrorHandler PushTo(Repository repo, Branch branch)
        {
            var options = new PushOptions
            {
                CredentialsProvider = (string _url, string _user, SupportedCredentialTypes _cred) => _usernamePasswordCredentials
            };
            var result = new ErrorHandler();
            try
            {
                var remote = repo.Network.Remotes["origin"];
                repo.Network.Push(remote, @$"refs/heads/{branch.FriendlyName}", options);
                //repo.Network.Push(branch, options);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = "Failed to push at " + branch.FriendlyName;
                result.Exception = ex;
                result.ExceptionMessage = ex.Message;
            }
            return result;
        }

        public string GetDiff(string fileName)
        {
            var sb = new StringBuilder();
            using (var repo = new Repository(LocalRepository))
            {
                var patch = repo.Diff.Compare<Patch>(new List<string>() { fileName });
                sb.Append(patch.Content);
            }
            return sb.ToString();
        }
    }
}
