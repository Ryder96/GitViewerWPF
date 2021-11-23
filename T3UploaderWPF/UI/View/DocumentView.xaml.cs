using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using T3UploaderWPF.UI.Model;
using T3UploaderWPF.UI.ModelView;

namespace T3UploaderWPF.UI.View
{
    public partial class DocumentView : Page
    {
        private readonly Document_MV _modelView;

        public DocumentView(Document_MV modelView)
        {
            _modelView = modelView;
            _modelView.PropertyChanged += ModelView_PropertyChanged;

            InitializeComponent();
        }

        private void ModelView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            fd_document.Blocks.Clear();
            if (!string.IsNullOrEmpty(e.PropertyName))
            {
                if (e.PropertyName.Equals("Content"))
                {

                    if (!_modelView.Content.StartsWith("diff"))
                    {
                        var run = new Run
                        {
                            Text = _modelView.Content
                        };
                        var paragraph = new Paragraph();
                        paragraph.Inlines.Add(run);
                        fd_document.Blocks.Add(paragraph);
                    }
                    else
                    {
                        var lines = _modelView.Content.Split("\n")[5..];
                        foreach (var line in lines)
                        {
                            var run = new Run();
                            var paragraph = new Paragraph();
                            if (line.StartsWith("+"))
                            {
                                run.Foreground = Brushes.ForestGreen;

                            }
                            else if (line.StartsWith("-"))
                            {
                                run.Foreground = Brushes.Red;
                            }
                            run.Text = line;
                            paragraph.Inlines.Add(run);
                            fd_document.Blocks.Add(paragraph);
                        }
                    }
                }
            }
        }
    }
}
