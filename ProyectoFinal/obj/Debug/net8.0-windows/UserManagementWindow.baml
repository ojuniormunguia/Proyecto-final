   M S B A M L   `   `   `  ����H  DProyectoFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:ProyectoFinalProyectoFinal  5   	   (    "ProyectoFinal.UserManagementWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:ProyectoFinal     5   	     � Title$  UserManagementWindow��5   	   $	��450��6&   $	��800��63    � 	Resources 5	      '   !ProyectoFinal.NullToBoolConverter   NullToBoolConverter��%   &	          5
   
   6$   5      .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��Gestión de Usuarios��6   $��24(�6/   $
��Bold"�6=   $��10q�6O   $��0��6[   � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $��3* �6#   �� 5      $��2* �6#   .+�5      �� $��0��6   $��10q�6)   .+�5      �� -   5        $��	SearchBox��5      $��Buscar��6+   
�������$��0,0,0,10q�6K   �� 5      ��6   �� *Users+ �� SelectedItem 68   �� *SelectedUser+��5      �� 5      .R�5       ��  �� Orientation$ 
Horizontal=�6)   $��5q�6B   .+�5!   "   �� $
��Bold"�6B   $��5q�6T   ��6,   �� *Name+6T   �� 5"   "   $��5q�6C   ��6,   �� *Email+6C   �� 5#   "   $��5q�6D   ��6,   �� *Gender+6D   5$      5%      5'      5(      �� 5+      $��1��6   $��10q�6)   .+�5,      �� $-��'También puede agregar un usuario nuevo��6   $��18(�6J   $
��Bold"�6X   $��0,0,0,10q�6j   �� 5-      $��Nombre��6   $
��Bold"�6*   �� 5.      $	��0,5q�68   ��6   �� *NewUser.Name+68   �� 5/      $��Nombre de Usuario��6   $
��Bold"�65   �� 50      $	��0,5q�6<   ��6   �� *NewUser.Username+6<   �� 51      $��Contraseña��6   $
��Bold"�6.   �� 52      $	��0,5q�6<   ��6   �� *NewUser.Password+6<   �� 53      $��Permisos��6   $
��Bold"�6,   �� 54      $	��0,5q�6H    6   �� *NewUser.Permissions+.@�55      �� $��Administrador��6#   �� 56      $��Usuario��6#   57      �� 58      $��Guardar��6   
���  ��
�������$	��200��6s   $��50��6   $
��0,10q�6�   ��6+   �� *SaveUserCommand+6�   �� 59      $��Eliminar��6   
���  ��
�������$	��200��6v   $��50��6�   $
��0,10q�6�   ��6,   �� *DeleteUserCommand+{�5:      �� *SelectedUser+ �� 	Converter# [  5;      5<      5=      5>      