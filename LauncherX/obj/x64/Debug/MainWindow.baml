   M S B A M L   `   `   `  ����D  @LauncherX, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LauncherX	LauncherX  5   	   D @FluentWPF, Version=0.8.0.6, Culture=neutral, PublicKeyToken=nullP6clr-namespace:SourceChord.FluentWPF;assembly=FluentWPFSourceChord.FluentWPF 5   	   h dMicrosoft.Toolkit.Wpf.UI.XamlHost, Version=6.1.0.0, Culture=neutral, PublicKeyToken=4aff67a105548ee2�Zclr-namespace:Microsoft.Toolkit.Wpf.UI.XamlHost;assembly=Microsoft.Toolkit.Wpf.UI.XamlHost!Microsoft.Toolkit.Wpf.UI.XamlHost 5   	       LauncherX.MainWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:LauncherX     5   	   Efw6clr-namespace:SourceChord.FluentWPF;assembly=FluentWPF     5   	   oxamlHostZclr-namespace:Microsoft.Toolkit.Wpf.UI.XamlHost;assembly=Microsoft.Toolkit.Wpf.UI.XamlHost     5   	   -   5        )  #SourceChord.FluentWPF.AcrylicWindow   Name$  window��5
   n    � Title$* $LauncherX - Organize all your stuff!��6	   $	��600��66   $	��830��6C   $	��400��6O   $	��650��6_      TintOpacity$
 0.75Q�6~      NoiseOpacity$
 0.01Q�6�    � WindowStyle$
 None=�6�      AcrylicWindowStyle$
 None=�6�   .��5      � -   5         �Name$ 	Container��5      $��0,0,0,0q�6   $��Stretch=�6/   $��Stretch=�6K   .+�5   
   � -   5        $
 grid��5      *  $SourceChord.FluentWPF.PointerTracker   Enabled . $��
0,90,0,0.5q�6>   $��Stretch=�6R   $��Stretch=�6p   
�������.+�5      7  1Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost  -   5          Name$ OpenFileHost��5   '   	   InitialTypeName$%	 Windows.UI.Xaml.Controls.Button��6=   $��	0,10,10,0q�6o   $��Right=�6�   $	��Top=�6�   $��32��6�   
 J� RenderTransformOrigin$
 0.5,0.5&�6�   $	��105��6�   6�   �� 5      -   5         ��Name$ header��5      $
��Left=�6(   $��	10,10,0,0q�6C   $
��Wrap=�6V   $%��My files, folders, and websites��6j   $	��Top=�6�   $
��Bold"�6�   $��16(�6�     5      -   5        $ gridviewhost��5   <   $��10,47,10,55q�6'   $'	 !Windows.UI.Xaml.Controls.GridView��6R   $��Stretch=�6�   $��Stretch=�6�   �� 5      -   5         ��Name$ SettingsButton��5      $��Right=�6-   $��40��6I   $��0,0,10,5q�6U   $��Bottom=�6g   $��40��6�   $��Segoe MDL2 Assets)�6�   $	����6�   $��19(�6�   ��6�   Z� 6�     5      -   5        $ OpenFolderHost��5   '   $��Right=�6?   $��32��6[   $��
0,10,125,0q�6g   $	��Top=�6{   $	��110��6�   $%	 Windows.UI.Xaml.Controls.Button��6�   � 5      -	   5        $ empty��5      $��Center=�6"   $��55��6?   $��0q�6K   $��Center=�6V   $	��350��6q   .+�5      �� -
   5         ��Name$ label1��5      $��There is nothing here.��6(   $
��Left=�6I   $	��Top=�6d   $��16(�6|   
�������$��40,0,0,0q�6�   �� 5      -   5        $ label��5      $	����6'   $��0q�6:   $
��Left=�6E   $��Center=�6`   $��24(�6{   $��Segoe MDL2 Assets)�6�   
�������$��31��6�   �� 5      -   5        $ label2��5      $@��:To add an item, click the buttons at the top right corner.��6(   $
��Left=�6m   $	��Top=�6�   
�������$��	40,25,0,0q�6�   5        5      -   5        $ OpenWebsiteHost��5   '   $��Right=�6@   $��32��6\   $��
0,10,245,0q�6h   $	��Top=�6|   $	��120��6�   $%	 Windows.UI.Xaml.Controls.Button��6�   6�   5        5   
   -   5        $ 
SearchHost��5   #   $��32��67   $��
50,53,50,0q�6C   $	��Top=�6W   $-	 'Windows.UI.Xaml.Controls.AutoSuggestBox��6o   6�   � 5!   
   -   5        $ TitleBar��5!   Q   $��32��6   $	��Top=�6   $��Stretch=�63   ���	#00000000.+�5"      �� -   5        $ CloseButton��5"      $	����6*   $��0q�6=   $��Center=�6H   $��42��6c   $��Segoe MDL2 Assets)�6�   $��32��6�   $��Right=�6�   $��11(�6�   ��6n   Z� ��6�   Z� 6�   �� 5#      -   5        $ MaximizeButton��5#      $	����6-   $��Right=�6@   $��0,0,42,0q�6\   $��Center=�6n   $��42��6�   $��Segoe MDL2 Assets)�6�   $��32��6�   $��11(�6	  ��6�   Z� ��6�   Z� 6	  �� 5$      -   5        $ MinimiseButton��5$      $	����6-   $��Right=�6@   $��0,0,84,0q�6\   $��Center=�6n   $��42��6�   $��Segoe MDL2 Assets)�6�   $��32��6�   $��11(�6	  ��6�   Z� ��6�   Z� 6	  �� 5%      -   5        $ TitleBarText��5%      $
��Left=�6.   $��30,0,0,0q�6I   $
��Wrap=�6[   $*��$LauncherX - Organize all your stuff!��6o   $��Center=�6�   �� 5&      -   5         ��Name$ TitleBarIcon��5&      $
��Left=�6*   $��16��6E   $��10,8,0,0q�6Q   $	��Top=�6c   $��16��6{   $��icon.ico��6�   5'      5)      5+      