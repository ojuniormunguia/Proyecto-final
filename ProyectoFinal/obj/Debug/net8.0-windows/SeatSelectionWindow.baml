   M S B A M L   `   `   `  ����  ProyectoFinal'    !ProyectoFinal.SeatSelectionWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Selección de Asientos��5   	   $	��600��6'   $
��1000��64   .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5
      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��Seleccionar Asientos��6   $��24(�60   $
��Bold"�6>   $��10q�6P   $��0��6\   �� 5   
   $��1��6   $��Center=�6"   $��Center=�6?   $��0,0,0,20q�6Z   .+�5      �� 
���   �$��20��6%   $	��800��61    �� RadiusX$ 10��6=    �� RadiusY$ 10��6J   �� 5      $��PANTALLA��6   $��16(�6(   $
��Bold"�66   
�������$��Center=�6[   5      �� 5   
   $��2��6   .��5      �� ��6   �� *Seats+��5      �� 5      .R�5      A�  A� Columns$ 12��6&   5      ��5      �� 5      .R�5       �� $��5q�5$   !   $��50��5%   !   $��50��5&   !   $��16(�5'   !   ��5    !   �� *
SeatNumber+��5!   !   �� *IsAvailable+   BoolToColorConverter �� 	Converter# [  ��5"   !   �� *DataContext.ToggleSeatCommand+ �� RelativeSource ��  �� AncestorType
 Window��5#   !   �� 5'   !   5(      5*      5+      �� 5-   
   $��4��6   $��10q�6"   $��Right=�6.   .+�5.      �� $��20(�6=   $
��Bold"�6K   ��6   �� *SelectedMovie.Title+6K   �� 5/      $��SALA A��6   $��16(�6&   �� 50      $��2D DUB��6   $��16(�6&   �� 51      $��4 de octubre 2024 22:00��6   $��16(�67   �� 52      $��Reservar��6   $	��200��6L   $��50��6X   
���  ��
���������6(   �� *ReservarCommand+6u   53      54      55      