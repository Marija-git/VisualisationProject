   M S B A M L   `   `   `  ����P  LProject_Visualisation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:Project_VisualisationProject_Visualisation  5   	   &     Project_Visualisation.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:Project_Visualisation     5   	     � Title$  
MainWindow��5   	   $��649.2��6   $��916.8��6+   .��5	      � .+�5   
   �� -   5         ��Name$ slider��5       � Minimum$ 1Q�6!    � Maximum$ 50Q�6-    � Value$ 1Q�6:   $��40,70,82.4,520q�6D   {�.  6]   �� 5   
   $
��Auto=�6   $
��Auto=�6<   $��1,87,22.8,6.8q�6_   .��5   
   � -   5        $��	GridPanel��5      $��38,44,55.8,25.8q�6!   $��649.2��6:   $��916.8��6I   .+�5      �� -   5        $��canvasDisplay��5       � Cursor$	 Pen��6*   
����ح�$��6,2,1.8,-4.8q�6O   5       � LayoutTransform 5      �� 5       �� ScaleX 5      ��  �� ElementName$ slider��	 �� Path$	 Value�
 �� ScaleY
 5      �� $ slider��$	 Value�5      5      o� 5   
   -   5        $��MenuButtons��5   "   
�������$
��Left=�65   $
��Auto��6P   $	��Top=�6^   $
��Auto��6v   .@�5      �� -   5         ��Name$ btnLoadModel��5      .��6G   Load entities6V   �� 5      -   5        $ btnConnection��5      .��6Q   Connections6^   �� 5      -   5        $ btnResistance��5      .��6P   
Resistance6\   �� 5       -   5        $ btnMaterial��5       .��6L   
Material6V   �� 5!      -	   5        $ btnSS��5!      .��6=   
ScreenShot6I   �� 5"      -
   5        $ btnHideInActive��5"      .��6A   Hide inactive6P   �� 5#      -   5        $ btnShowInActive��5#      .��6E   Show inacitve6T   �� 5$      -   5        $ 
btnEllipse��5$      .��6?   	Ellipse6H   �� 5%      -   5        $ 
btnPolygon��5%      .��6?   	Polygon6H   �� 5&      -   5        $ 
btnAddText��5&      .��6?   
Add Text6I   �� 5'      -   5        $ btnUndo��5'      .��69   Undo6?   �� 5(      -   5        $ btnClear��5(      .��6;   Clear6B   �� 5)      -   5        $ btnRedo��5)      .��69   Redo6?   5*      5,      5-      