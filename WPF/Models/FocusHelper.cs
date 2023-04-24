using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using BIMicon.BIMiconToolbar.WPF.UserControls.MultiSelectionTreeViewControl;

namespace BIMicon.BIMiconToolbar.WPF.Models
{
    public static class FocusHelper
    {
        #region Public methods

        public static void Focus(EditTextBox element)
        {
            //System.Diagnostics.Debug.WriteLine("Focus textbox with helper:" + element.Text);
            FocusCore(element);
            element.BringIntoView();
        }

        public static void Focus(MultiSelectionTreeViewItem element, bool bringIntoView = false)
        {
            //System.Diagnostics.Debug.WriteLine("FocusHelper focusing " + (bringIntoView ? "[into view] " : "") + element.DataContext);
            FocusCore(element);

            if (bringIntoView)
            {
                FrameworkElement itemContent = (FrameworkElement)element.Template.FindName("headerBorder", element);
                if (itemContent != null)   // May not be rendered yet...
                {
                    itemContent.BringIntoView();
                }
            }
        }

        public static void Focus(MultiSelectionTreeView element)
        {
            //System.Diagnostics.Debug.WriteLine("Focus Tree with helper");
            FocusCore(element);
            element.BringIntoView();
        }

        private static void FocusCore(FrameworkElement element)
        {
            //System.Diagnostics.Debug.WriteLine("Focusing element " + element.ToString());
            //System.Diagnostics.Debug.WriteLine(Environment.StackTrace);
            if (!element.Focus())
            {
                //System.Diagnostics.Debug.WriteLine("- Element could not be focused, invoking in dispatcher thread");
                element.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() => element.Focus()));
            }

#if DEBUG
            // no good idea, seems to block sometimes
            int i = 0;
            while (i < 5)
            {
                if (element.IsFocused)
                {
                    //if (i > 0)
                    //    System.Diagnostics.Debug.WriteLine("- Element is focused now in round " + i + ", leaving");
                    return;
                }
                Thread.Sleep(20);
                i++;
            }
            //if (i >= 5)
            //{
            //    System.Diagnostics.Debug.WriteLine("- Element is not focused after 500 ms, giving up");
            //}
#endif
        }

        #endregion Public methods
    }
    
}
