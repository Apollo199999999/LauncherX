   M S B A M L   `   `   `  ����D  @LauncherX, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LauncherX	LauncherX  5          LauncherX.WebsiteDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      )localclr-namespace:LauncherX     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      A =Wpf.Ui, Version=2.0.3.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5        � Title$  Add a website��5	      $	��500��5
      $	��180��5      $��0��5        Wpf.Ui.Controls.UiWindow!   ExtendsContentIntoTitleBar .    WindowBackdropType$
 Mica=�5         WindowCornerPreference$ Round=�5       � 
ResizeMode$ NoResize=�5         UiWindow#��[  5       � WindowStartupLocation$ CenterScreen=�5      .��5      �    ControlFillColorDefaultBrush#��[ 6   .+�5   
     Wpf.Ui.Controls.TitleBar     Title$ Add a website��5         Icon$ icon.ico��5         ShowMaximize .  	   ShowMinimize	 .  
   UseSnapLayout
 . 5      �� 5   
   -   5         ��Name$ label��5      $��	15,55,0,0q�5      $
��Left=�5      $	��Top=�5       $
��URL:��5!        Wpf.Ui.Controls.TextBox  5"   
   -   5          Name$ urlBox��5#      $��15,81,15,10q�5$      $	��Top=�5%         PlaceholderEnabled .    PlaceholderText$ Enter website URL here��5'        Wpf.Ui.Controls.Button  5(   
   -   5          Name$ OkBtn��5)      $��85��5*      $��32��5+      $��0,131,110,0q�5,      $��Right=�5-      $	��Top=�5.         
Appearance$ Primary=�5/      $��OK��51        53   
   -   5        $ 	CancelBtn��54      $��85��55      $��32��56      $��
0,131,15,0q�57      $��Right=�58      $	��Top=�59      $��Cancel��5;      5<      5=      