   M S B A M L   `   `   `  ����C  ?ClientXO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:ClientXOClientXO  5   	       ClientXO.Connect   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:ClientXO     5   	     � Title$  Подключиться��5   	   $	��150��5	   	   $	��360��6    � WindowStartupLocation$ CenterScreen=�5
   	    � 
ResizeMode$ NoResize=�6.   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      $
��auto �6   �� 5      .+�5   
   �� .��6   .,Введите IP адрес сервера:6+   �� 5   
   $��Center=�6   $��1��62    �� Orientation$ 
Horizontal=�6B   .+�5      �� -   5         ��Name$ 
textBoxIP1��5      $��2q�6*   $��30��65   $��Center=�6@   .��6d   1276i   �� 5      .��6   .6   �� 5      -   5        $ 
textBoxIP2��5      $��2q�6*   $��30��65   $��Center=�6@   .��6d   06g   �� 5      .��6   .6   �� 5      -   5        $ 
textBoxIP3��5      $��2q�6*   $��30��65   $��Center=�6@   .��6d   06g   �� 5      .��6   .6   �� 5       -   5        $ 
textBoxIP4��5       $��2q�6*   $��30��65   $��Center=�6@   .��6d   16g   5!      �� 5$   
   $��1��6   .��6   1/Введите Порт подключения:68   �� 5%   
   -   5        $ textBoxPort��5%      $��1��6'   $��1��64   $��3,2,2,2q�6E   $��40��6V   $
��Left=�6a   $��Center=�6|   .��6�   70186�   �� 5(   
   $��2��6   .��6   !Введите Ваше имя:60   �� 5)   
   -   5        $ textBoxName��5)      $��2��6'   $��1��64   $��3,2,0,2q�6E   $
��Left=�6V   $
��Left=�6q   $��Center=�6�   $	��182��6�   .��6�   Дмитрий6�   �� 5+   
   -   5        $��2q�5+      $��3��6   $��2��6)    �� 	IsDefault . .��6f   Подключиться6t   5-      5.      