   M S B A M L   `   `   `  ����H  DPhoto3DEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Photo3DEditor.ViewsPhoto3DEditor.Views  5   	   ?$clr-namespace:Photo3DEditor.ServicesPhoto3DEditor.Services  5   	   %    Photo3DEditor.Views.ImageWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   3local!clr-namespace:Photo3DEditor.Views     5   	   9services$clr-namespace:Photo3DEditor.Services     5   	     � Title$  Image Editor��5   	   $	��600��6   $	��800��6+    � 	Resources 5      6   0Photo3DEditor.Services.NullToVisibilityConverter   NullToVisibilityConverter��%   &	          5   
   6-   5	      .��5
      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   
�������$��1q�61   ._�5      �� -   5         ��Name$ ImageCanvas��5      
������ .+�5      �� -   5         ��Name$ 	MainImage��5      $%��C:/Users/461/Downloads/aded.jpg��5      $��Uniform=�5      $	��716��5      $	��524��5       J� IsHitTestVisible$ False��5       K� ZIndex$ 0��5      �� 5      -   5         ��Name$ TrianglesControl��5      $ 1��6[   ��69   �� *	Triangles+��5      �� *ActualWidth+ �� ElementName$ ImageCanvas����5       �� *ActualHeight+$ ImageCanvas����5!      �� 5"      .R�5#      �� ��6%   �� *ActualWidth+$ ImageCanvas����5$      �� *ActualHeight+$ ImageCanvas��5%      ��5'      �� 5(      .R�5)      � 
���/���$��1��5+      ���	#20ADD8E6	 � Points	 5)   &   �� *
PathPoints+5,      5-      5/      �� 52      -   5        $ PointsControl��52      ��66   �� *Points+��53      �� 54      .R�55      �� 56      ��58      �� 59      
 �� 
TargetType"
 ��6    .�5:      ��  �� Property �@��Left �� Value 6<   �� *X+�� 5;       �@��Top 6;   �� *Y+5<      ��5>      �� 5?      .R�5@      A� $��12��6&   $��12��61   
���/���6=   5A      5C      �� 5F      -   5        $ SelectedPointControl��5F      ��5G      �� *SelectedPointAsEnumerable+��5H      �� 5I      .R�5J      �� 5K      ��5M      �� 5N      "
 ��6    .�5O      ��  �@��Left 6<   �� *X+�� 5P       �@��Top 6;   �� *Y+�� 5Q       � y� 6;   �� *.+ �� 	Converter# [  5R      ��5T      �� 5U      .R�5V      A� $��14��6&   $��14��61   
����  �
���  ��$��2��6Z   5W      5Y      5Z      5[      �� 5^   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   .+�5_      �� -   5        $��
Load Image��5_      $��5q�6B   $��10q�6M   �� 5`      -   5        $��Экспорт в .obj��5`      $��10q�5b   	   $��5q�5c   	   $��Right=�5d   	   �� 5e      -   5        $$��Импортировать OBJ��5e      61   5f      5g      5h      