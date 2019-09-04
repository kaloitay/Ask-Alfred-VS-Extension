using System.Windows;
using System.Windows.Controls;
using Ask_Alfred.Objects;
using Ask_Alfred.Infrastructure.Interfaces;
using System;

namespace Ask_Alfred.UI
{
    public class AskAlfredResultUIElement
    {
        private readonly string DATE_FORMAT = "dd/MM/yyyy";

        public DockPanel dockPanel { get; private set; }
        private StackPanel websiteNameStackPanel { get; set; }
        private Image resultImage { get; set; }
        private Image addtionalInfoImage { get; set; }
        private TextBlock dateTextBlock { get; set; }
        private TextBlock websiteNameTextBlock { get; set; }
        private TextBlock subjectTextBlock { get; set; }
        public AskAlfredResultUIElement(IPage i_Page, ResourceDictionary i_Resources)
        {
            createControls(i_Page);
            setControlsText(i_Page);
            setControlsStyle(i_Resources);
            setControlsToolTip();
            setControlsRelations();
        }

        private void setControlsToolTip()
        {
            dockPanel.ToolTip = subjectTextBlock.Text;
        }

        private void createAddtionalInfoImage(IPage i_Page)
        {
            if (i_Page is StackoverflowPage)
            {
                StackoverflowPage stackoverflowPage = i_Page as StackoverflowPage;

                if (stackoverflowPage.IsAnswered) // TODO: need to fix this condition to green v condition
                {
                    addtionalInfoImage = new Image();
                }
            }
        }

        private void setControlsStyle(ResourceDictionary i_Resources)
        {
            dateTextBlock.Style = (Style)(i_Resources["DateTextBlock"]);
            subjectTextBlock.Style = (Style)(i_Resources["SubjectTextBlock"]);
            websiteNameTextBlock.Style = (Style)(i_Resources["WebsiteNameTextBlock"]);
            websiteNameStackPanel.Style = (Style)(i_Resources["WebsiteNameStackPanel"]);
            resultImage.Style = (Style)(i_Resources["ResultImage"]);

            if (addtionalInfoImage != null)
            {
                addtionalInfoImage.Style = (Style)(i_Resources["AddtionalInfoImage"]);
            }
        }

        private void setControlsRelations()
        {
            dockPanel.Children.Add(resultImage);
            dockPanel.Children.Add(dateTextBlock);
            dockPanel.Children.Add(websiteNameStackPanel);
            dockPanel.Children.Add(subjectTextBlock);

            if (addtionalInfoImage != null)
            {
                websiteNameStackPanel.Children.Add(addtionalInfoImage);
            }

            websiteNameStackPanel.Children.Add(websiteNameTextBlock);
        }

        private void setControlsText(IPage i_Page)
        {
            dockPanel.Tag = i_Page.Url;
            dateTextBlock.Text = i_Page.Date.ToString(DATE_FORMAT);
            websiteNameTextBlock.Text = i_Page.WebsiteName;
            subjectTextBlock.Text = i_Page.Subject;
        }

        private void createControls(IPage i_Page)
        {
            dockPanel = new DockPanel();
            dateTextBlock = new TextBlock();
            websiteNameStackPanel = new StackPanel();
            websiteNameTextBlock = new TextBlock();
            subjectTextBlock = new TextBlock();
            resultImage = new Image();

            createAddtionalInfoImage(i_Page);
        }
    }
}