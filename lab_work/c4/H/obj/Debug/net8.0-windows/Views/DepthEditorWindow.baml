   M S B A M L   `   `   `  ����  Photo3DEditor+    %Photo3DEditor.Views.DepthEditorWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Depth Editor��5   	   $	��450��6   $	��800��6+   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��200 �6   �� 5	      $��* �6   .+�5   
   �� -   5         ��Name$ 
PointsList��5       �� DisplayMemberPath$ DisplayInfo��5   
   $��5q�5   
   ��5   
   ��  �� SelectedItem 5   
   �� *SelectedPoint+ �� Mode$ TwoWay=�5   
   �� 5   
   $��1��6   $��10q�6%   .+�5      �� $��Z Coordinate:��6   $��14(�6-   �� 5      -   5         ��Name$ ZSlider��5       � Minimum$
 -100Q�5       � Maximum$	 100Q�5      $
��0 10q�5      	 � Value	 5      �� *SelectedItem.Z+
 �� ElementName$
 
PointsList��$ TwoWay=��� 5      -   5        $��Обновить 3D��5      $��10q�5      $��	0 20 10 0q�5      $��Right=�5       �� 5"      -   5        $��Удалить точку��5"      $��10q�5$      $��	0 10 10 0q�5%      $��Right=�5&      5(      5)      5*      