using System.Windows;
using System.Windows.Controls;
using Ask_Alfred.Objects;
using Ask_Alfred.Infrastructure.Interfaces;
using System;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace Ask_Alfred.UI
{
    public class AskAlfredResultUIElement
    {
        private readonly string DATE_FORMAT = "dd/MM/yyyy";

        public DockPanel resultRootDockPanel { get; private set; }
        public GridSplitter gridSplitter { get; private set; }
        private StackPanel websiteNameStackPanel { get; set; }
        private Image resultImage { get; set; }
        private Image addtionalInfoImage { get; set; }
        private TextBlock resultDateTextBlock { get; set; }
        private TextBlock websiteNameTextBlock { get; set; }
        private TextBlock resultSubjectTextBlock { get; set; }
        public AskAlfredResultUIElement(IPage i_Page, ResourceDictionary i_Resources)
        {
            createControls();
            // setImages();
            setControlsText(i_Page);
            setControlsStyle(i_Resources);
            setControlsRelations();

            //if (i_Page is StackoverflowPage)
            //{
            //    StackoverflowPage stackoverflowPage = i_Page as StackoverflowPage;

            //    if (stackoverflowPage.IsAnswered) // TODO: need to fix this condition to green v condition
            //    {
            //        addtionalInfoImage = new Image();
            //        addtionalInfoImage.Height = 7;
            //        addtionalInfoImage.Width = 7;
            //        websiteNameStackPanel.Children.Add(addtionalInfoImage);
            //    }
            //}
        }

        private void setControlsStyle(ResourceDictionary i_Resources)
        {
            resultRootDockPanel.Style = (Style)(i_Resources["ResultRootDockPanel"]);
            resultDateTextBlock.Style = (Style)(i_Resources["DateTextBlock"]);
            resultSubjectTextBlock.Style = (Style)(i_Resources["SubjectTextBlock"]);
            websiteNameTextBlock.Style = (Style)(i_Resources["WebsiteNameTextBlock"]);
            websiteNameStackPanel.Style = (Style)(i_Resources["WebsiteNameStackPanel"]);
        }

        private void setImages()
        {
            resultImage.Source = new BitmapImage(new Uri(@"\Resources\Icons\go_to_web_icon.png", UriKind.Relative));
            addtionalInfoImage.Source = new BitmapImage(new Uri(@"\Resources\Icons\green_checkmark_stackoverflow_icon.png", UriKind.Relative));
        }

        private void setControlsRelations()
        {
            websiteNameStackPanel.Children.Add(websiteNameTextBlock);

            resultRootDockPanel.Children.Add(resultImage);
            resultRootDockPanel.Children.Add(resultDateTextBlock);
            resultRootDockPanel.Children.Add(websiteNameStackPanel);
            resultRootDockPanel.Children.Add(resultSubjectTextBlock);

        }

        private void setControlsText(IPage page)
        {
            resultDateTextBlock.Text = page.Date.ToString(DATE_FORMAT);
            websiteNameTextBlock.Text = page.WebsiteName;
            resultSubjectTextBlock.Text = page.Subject;
        }

        private void createControls()
        {
            resultRootDockPanel = new DockPanel();
            resultDateTextBlock = new TextBlock();
            websiteNameStackPanel = new StackPanel();
            websiteNameTextBlock = new TextBlock();
            resultSubjectTextBlock = new TextBlock();
            gridSplitter = new GridSplitter();
            resultImage = new Image();

            resultImage.Width = 40;
        }
    }
}