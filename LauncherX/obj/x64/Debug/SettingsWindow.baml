   M S B A M L   `   `   `  ����D  @LauncherX, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LauncherX	LauncherX  5   	   h dMicrosoft.Toolkit.Wpf.UI.XamlHost, Version=6.1.0.0, Culture=neutral, PublicKeyToken=4aff67a105548ee2�Zclr-namespace:Microsoft.Toolkit.Wpf.UI.XamlHost;assembly=Microsoft.Toolkit.Wpf.UI.XamlHost!Microsoft.Toolkit.Wpf.UI.XamlHost 5   	   D @FluentWPF, Version=0.8.0.6, Culture=neutral, PublicKeyToken=nullP6clr-namespace:SourceChord.FluentWPF;assembly=FluentWPFSourceChord.FluentWPF 5
   	       LauncherX.SettingsWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:LauncherX     5   	   oXamlHostZclr-namespace:Microsoft.Toolkit.Wpf.UI.XamlHost;assembly=Microsoft.Toolkit.Wpf.UI.XamlHost     5   	   Efw6clr-namespace:SourceChord.FluentWPF;assembly=FluentWPF     5
   	   -   5          � Title$  Settings��5   	   $	��490��6   $	��355��6'    � 
ResizeMode$ NoResize=�63   )  #SourceChord.FluentWPF.AcrylicWindow   AcrylicWindowStyle$
 None=�6I      TintOpacity$ 2Q�6c      NoiseOpacity$ 0Q�6s    � WindowStyle$
 None=�6�   .��5      � $��0,0,0,0q�6   .+�5   
   �� -   5         ��Name$ 	textBlock��5      $
��Left=�6'   $��	10,10,0,0q�6B   $
��Wrap=�6U   $��Settings��6i   $	��Top=�6y   $��16(�6�   $
��Bold"�6�   �� 5   
   -   5        $ AboutHeader��5      $
��Left=�6)   $��
10,190,0,0q�6D   $
��Wrap=�6X   $��About��6l   $	��Top=�6y   $��16(�6�   $
��Bold"�6�   7  1Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost  5   
   -   5          Name$ scalescrollhost��5   #      InitialTypeName$% Windows.UI.Xaml.Controls.Slider��6<   $
��Left=�6n   $��40��6�   $��	10,60,0,0q�6�   $	��Top=�6�   $	��254��6�   6�   �� 5   
   -   5        $ changeScaleTextHeader��5      $
��Left=�63   $��	10,45,0,0q�6N   $
��Wrap=�6a   $��Change Icon Size��6u   $	��Top=�6�   $��12(�6�   $��Normal"�6�   �� 5   
   -   5        	 ��Name$	 CloseButton��5      $��Save and Exit��6&   $��Right=�6>   $��
0,0,10,5.5q�6Z   $��Bottom=�6n   $	��100��6�   $��30��6�   $��13(�6�   6�   �� 5   
   -   5        $ changeHeaderTextHeader��5      $
��Left=�64   $��
10,115,0,0q�6O   $
��Wrap=�6c   $��Change Header Text��6w   $	��Top=�6�   $��12(�6�   $��Normal"�6�   �� 5   
   -   5        
 ��Name$
 changeHeaderTextTextBox��5      $
��Left=�63   $��25��6N   $��
10,135,0,0q�6Z    �� TextWrapping$
 Wrap=�6n   $	��Top=�6�   $	��335��6�   $��13(�6�   6�   �� 5   
   -	   5        $ 
textBlock2��5      $��Center=�6(   $��	0,256,0,0q�6E   $
��Wrap=�6X   $��Version 1.0.1��6l   $	��Top=�6�   �� 5   
   -
   5        $ 
textBlock3��5      $��Center=�6(   $��	0,276,0,0q�6E   $
��Wrap=�6X   $*��$Copyright © 2020-present ClickPhase��6l   $	��Top=�6�    � Cursor$  ��6�   � 5   
   -   5         �Name$ LogoContainer��5      $��Center=�6&   $��32��6C   $��	0,215,0,0q�6O   $	��Top=�6b   $	��172��6z   .+�5      �� -   5         ��Name$ image��5      $
��Left=�6#   $��32��6>   $��0q�6J   $	��Top=�6U   $��32��6m   $��icon.ico��6x   �� 5      -   5        $ 
textBlock1��5      $
��Left=�6,   $��42,0,0,0q�6G   $
��Wrap=�6Y   $��	LauncherX��6m   $	��Top=�6~   $��24(�6�   $
��Bold"�6�   $	��130��6�   5      �� 5   
   -   5        $ 
textBlock4��5      $��Center=�6(   $��	0,296,0,0q�6E   $
��Wrap=�6X   $q��kThis project is licensed under the MIT Licence. For more information, visit LauncherX's Github respository.��6l   $	��Top=�6�    �� TextAlignment$ Center=�6�   �� 5   
   -   5        $ 
textBlock5��5      $��Center=�6(   $��28,375,27,0q�6E   $
��Wrap=�6Z   $����Hi! I am Matthias, the (only) developer of LauncherX. This is my first actual program for Windows, so it may not be perfect. Anyways, I hope that you enjoy using LauncherX.��6n   $	��Top=�6"  $ Center=�6:  $	��300��6Q  �� 5   
   -   5        $	 VisitGithub��5      $*��$Visit LauncherX's Github respository��6&   $��Center=�6U   $��	0,335,0,0q�6r   $	��Top=�6�   $	��220��6�   $��30��6�   $��13(�6�   5!      5#      