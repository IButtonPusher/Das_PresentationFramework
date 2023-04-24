using System;
using System.Threading.Tasks;
using System.Windows;

namespace Das.Views.Wpf;

public class StateRestrictedWindow : Window
{
   public Boolean CanMaximizeWindow
   {
      get => (Boolean)GetValue(CanMaximizeWindowProperty);
      set => SetValue(CanMaximizeWindowProperty, value);
   }

   public Boolean CanMinimizeWindow
   {
      get => (Boolean)GetValue(CanMinimizeWindowProperty);
      set => SetValue(CanMinimizeWindowProperty, value);
   }

   public Boolean CanCloseWindow
   {
      get => (Boolean)GetValue(CanCloseWindowProperty);
      set => SetValue(CanCloseWindowProperty, value);
   }

   public static readonly DependencyProperty CanMaximizeWindowProperty =
      DependencyProperty.Register(
         nameof(CanMaximizeWindow),
         typeof(Boolean),
         typeof(StateRestrictedWindow),
         new PropertyMetadata(true));

   public static readonly DependencyProperty CanMinimizeWindowProperty =
      DependencyProperty.Register(
         nameof(CanMinimizeWindow),
         typeof(Boolean),
         typeof(StateRestrictedWindow),
         new PropertyMetadata(true));

   public static readonly DependencyProperty CanCloseWindowProperty =
      DependencyProperty.Register(
         nameof(CanCloseWindow),
         typeof(Boolean),
         typeof(StateRestrictedWindow),
         new PropertyMetadata(true));
}