﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GongSolutions.Wpf.DragDrop
{
  public static partial class DragDrop
  {
    public static DataFormat DataFormat { get; } = DataFormats.GetDataFormat("GongSolutions.Wpf.DragDrop");

    /// <summary>
    /// Gets or Sets whether the control can be used as drag source.
    /// </summary>
    public static readonly DependencyProperty IsDragSourceProperty
      = DependencyProperty.RegisterAttached("IsDragSource",
                                            typeof(bool),
                                            typeof(DragDrop),
                                            new UIPropertyMetadata(false, IsDragSourceChanged));

    /// <summary>
    /// Gets whether the control can be used as drag source.
    /// </summary>
    public static bool GetIsDragSource(UIElement target)
    {
      return (bool)target.GetValue(IsDragSourceProperty);
    }

    /// <summary>
    /// Sets whether the control can be used as drag source.
    /// </summary>
    public static void SetIsDragSource(UIElement target, bool value)
    {
      target.SetValue(IsDragSourceProperty, value);
    }

    private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var uiElement = (UIElement)d;

      if ((bool)e.NewValue)
      {
        uiElement.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
        uiElement.PreviewMouseLeftButtonUp += DragSource_PreviewMouseLeftButtonUp;
        uiElement.PreviewMouseMove += DragSource_PreviewMouseMove;
        uiElement.QueryContinueDrag += DragSource_QueryContinueDrag;
      }
      else
      {
        uiElement.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
        uiElement.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
        uiElement.PreviewMouseMove -= DragSource_PreviewMouseMove;
        uiElement.QueryContinueDrag -= DragSource_QueryContinueDrag;
      }
    }

    /// <summary>
    /// Gets or Sets whether the control can be used as drop target.
    /// </summary>
    public static readonly DependencyProperty IsDropTargetProperty
      = DependencyProperty.RegisterAttached("IsDropTarget",
                                            typeof(bool),
                                            typeof(DragDrop),
                                            new UIPropertyMetadata(false, IsDropTargetChanged));

    /// <summary>
    /// Gets whether the control can be used as drop target.
    /// </summary>
    public static bool GetIsDropTarget(UIElement target)
    {
      return (bool)target.GetValue(IsDropTargetProperty);
    }

    /// <summary>
    /// Sets whether the control can be used as drop target.
    /// </summary>
    public static void SetIsDropTarget(UIElement target, bool value)
    {
      target.SetValue(IsDropTargetProperty, value);
    }

    private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var uiElement = (UIElement)d;

      if ((bool)e.NewValue)
      {
        uiElement.AllowDrop = true;

        if (uiElement is ItemsControl)
        {
          // use normal events for ItemsControls
          uiElement.DragEnter += DropTarget_PreviewDragEnter;
          uiElement.DragLeave += DropTarget_PreviewDragLeave;
          uiElement.DragOver += DropTarget_PreviewDragOver;
          uiElement.Drop += DropTarget_PreviewDrop;
          uiElement.GiveFeedback += DropTarget_GiveFeedback;
        }
        else
        {
          // issue #85: try using preview events for all other elements than ItemsControls
          uiElement.PreviewDragEnter += DropTarget_PreviewDragEnter;
          uiElement.PreviewDragLeave += DropTarget_PreviewDragLeave;
          uiElement.PreviewDragOver += DropTarget_PreviewDragOver;
          uiElement.PreviewDrop += DropTarget_PreviewDrop;
          uiElement.PreviewGiveFeedback += DropTarget_GiveFeedback;
        }
      }
      else
      {
        uiElement.AllowDrop = false;

        if (uiElement is ItemsControl)
        {
          uiElement.DragEnter -= DropTarget_PreviewDragEnter;
          uiElement.DragLeave -= DropTarget_PreviewDragLeave;
          uiElement.DragOver -= DropTarget_PreviewDragOver;
          uiElement.Drop -= DropTarget_PreviewDrop;
          uiElement.GiveFeedback -= DropTarget_GiveFeedback;
        }
        else
        {
          uiElement.PreviewDragEnter -= DropTarget_PreviewDragEnter;
          uiElement.PreviewDragLeave -= DropTarget_PreviewDragLeave;
          uiElement.PreviewDragOver -= DropTarget_PreviewDragOver;
          uiElement.PreviewDrop -= DropTarget_PreviewDrop;
          uiElement.PreviewGiveFeedback -= DropTarget_GiveFeedback;
        }

        Mouse.OverrideCursor = null;
      }
    }

    /// <summary>
    /// Gets the default DragHandler.
    /// </summary>
    public static IDragSource DefaultDragHandler { get; } = new DefaultDragHandler();

    /// <summary>
    /// Gets the default DropHandler.
    /// </summary>
    public static IDropTarget DefaultDropHandler { get; } = new DefaultDropHandler();

    /// <summary>
    /// Gets or Sets the handler for the drag action.
    /// </summary>
    public static readonly DependencyProperty DragHandlerProperty
      = DependencyProperty.RegisterAttached("DragHandler",
                                            typeof(IDragSource),
                                            typeof(DragDrop));

    /// <summary>
    /// Gets the handler for the drag action.
    /// </summary>
    public static IDragSource GetDragHandler(UIElement target)
    {
      return (IDragSource)target.GetValue(DragHandlerProperty);
    }

    /// <summary>
    /// Sets the handler for the drag action.
    /// </summary>
    public static void SetDragHandler(UIElement target, IDragSource value)
    {
      target.SetValue(DragHandlerProperty, value);
    }

    /// <summary>
    /// Gets or Sets the handler for the drop action.
    /// </summary>
    public static readonly DependencyProperty DropHandlerProperty
      = DependencyProperty.RegisterAttached("DropHandler",
                                            typeof(IDropTarget),
                                            typeof(DragDrop));

    /// <summary>
    /// Gets the handler for the drop action.
    /// </summary>
    public static IDropTarget GetDropHandler(UIElement target)
    {
      return (IDropTarget)target.GetValue(DropHandlerProperty);
    }

    /// <summary>
    /// Sets the handler for the drop action.
    /// </summary>
    public static void SetDropHandler(UIElement target, IDropTarget value)
    {
      target.SetValue(DropHandlerProperty, value);
    }

    /// <summary>
    /// Gets or Sets the ScrollingMode for the drop action.
    /// </summary>
    public static readonly DependencyProperty DropScrollingModeProperty
      = DependencyProperty.RegisterAttached("DropScrollingMode",
                                            typeof(ScrollingMode),
                                            typeof(DragDrop),
                                            new PropertyMetadata(ScrollingMode.Both));

    /// <summary>
    /// Gets the ScrollingMode for the drop action.
    /// </summary>
    public static ScrollingMode GetDropScrollingMode(UIElement target)
    {
      return (ScrollingMode)target.GetValue(DropScrollingModeProperty);
    }

    /// <summary>
    /// Sets the ScrollingMode for the drop action.
    /// </summary>
    public static void SetDropScrollingMode(UIElement target, ScrollingMode value)
    {
      target.SetValue(DropScrollingModeProperty, value);
    }

    /// <summary>
    /// Gets or Sets a context for a control. Only controls with the same context are allowed for drag or drop actions.
    /// </summary>
    public static readonly DependencyProperty DragDropContextProperty
      = DependencyProperty.RegisterAttached("DragDropContext",
                                            typeof(string),
                                            typeof(DragDrop),
                                            new UIPropertyMetadata(string.Empty));

    /// <summary>
    /// Gets a context for a control. Only controls with the same context are allowed for drag or drop actions.
    /// </summary>
    public static string GetDragDropContext(UIElement target)
    {
      return (string)target.GetValue(DragDropContextProperty);
    }

    /// <summary>
    /// Sets a context for a control. Only controls with the same context are allowed for drag or drop actions.
    /// </summary>
    public static void SetDragDropContext(UIElement target, string value)
    {
      target.SetValue(DragDropContextProperty, value);
    }

    /// <summary>
    /// Gets or Sets whether an element under the mouse should be ignored for the drag action.
    /// </summary>
    public static readonly DependencyProperty DragSourceIgnoreProperty
      = DependencyProperty.RegisterAttached("DragSourceIgnore",
                                            typeof(bool),
                                            typeof(DragDrop),
                                            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Gets whether an element under the mouse should be ignored for the drag action.
    /// </summary>
    public static bool GetDragSourceIgnore(UIElement source)
    {
      return (bool)source.GetValue(DragSourceIgnoreProperty);
    }

    /// <summary>
    /// Sets whether an element under the mouse should be ignored for the drag action.
    /// </summary>
    public static void SetDragSourceIgnore(UIElement source, bool value)
    {
      source.SetValue(DragSourceIgnoreProperty, value);
    }

    /// <summary>
    /// Gets or Sets wheter the drag action should be started only directly on a selected item
    /// or also on the free control space (e.g. in a ListBox).
    /// </summary>
    public static readonly DependencyProperty DragDirectlySelectedOnlyProperty
      = DependencyProperty.RegisterAttached("DragDirectlySelectedOnly",
                                            typeof(bool),
                                            typeof(DragDrop),
                                            new PropertyMetadata(false));

    /// <summary>
    /// Gets wheter the drag action should be started only directly on a selected item.
    /// </summary>
    public static bool GetDragDirectlySelectedOnly(DependencyObject obj)
    {
      return (bool)obj.GetValue(DragDirectlySelectedOnlyProperty);
    }

    /// <summary>
    /// Sets wheter the drag action should be started only directly on a selected item.
    /// </summary>
    public static void SetDragDirectlySelectedOnly(DependencyObject obj, bool value)
    {
      obj.SetValue(DragDirectlySelectedOnlyProperty, value);
    }

    /// <summary>
    /// The drag drop copy key state property (default None).
    /// So the drag drop action is
    /// - Move, within the same control or from one to another, if the drag drop key state is None
    /// - Copy, from one to another control with the given drag drop copy key state
    /// </summary>
    public static readonly DependencyProperty DragDropCopyKeyStateProperty
      = DependencyProperty.RegisterAttached("DragDropCopyKeyState",
                                            typeof(DragDropKeyStates),
                                            typeof(DragDrop),
                                            new PropertyMetadata(default(DragDropKeyStates)));

    /// <summary>
    /// Gets the copy key state which indicates the effect of the drag drop operation.
    /// </summary>
    public static DragDropKeyStates GetDragDropCopyKeyState(UIElement target)
    {
      return (DragDropKeyStates)target.GetValue(DragDropCopyKeyStateProperty);
    }

    /// <summary>
    /// Sets the copy key state which indicates the effect of the drag drop operation.
    /// </summary>
    public static void SetDragDropCopyKeyState(UIElement target, DragDropKeyStates value)
    {
      target.SetValue(DragDropCopyKeyStateProperty, value);
    }

    /// <summary>
    /// Gets or Sets whether if the default DragAdorner should be use.
    /// </summary>
    public static readonly DependencyProperty UseDefaultDragAdornerProperty
      = DependencyProperty.RegisterAttached("UseDefaultDragAdorner",
                                            typeof(bool),
                                            typeof(DragDrop),
                                            new PropertyMetadata(false));

    /// <summary>
    /// Gets whether if the default DragAdorner is used.
    /// </summary>
    public static bool GetUseDefaultDragAdorner(UIElement target)
    {
      return (bool)target.GetValue(UseDefaultDragAdornerProperty);
    }

    /// <summary>
    /// Sets whether if the default DragAdorner should be use.
    /// </summary>
    public static void SetUseDefaultDragAdorner(UIElement target, bool value)
    {
      target.SetValue(UseDefaultDragAdornerProperty, value);
    }

    /// <summary>
    /// Gets or Sets the opacity of the default DragAdorner.
    /// </summary>
    public static readonly DependencyProperty DefaultDragAdornerOpacityProperty
      = DependencyProperty.RegisterAttached("DefaultDragAdornerOpacity",
                                            typeof(double),
                                            typeof(DragDrop),
                                            new PropertyMetadata(0.8));

    /// <summary>
    /// Gets the opacity of the default DragAdorner.
    /// </summary>
    public static double GetDefaultDragAdornerOpacity(UIElement target)
    {
      return (double)target.GetValue(DefaultDragAdornerOpacityProperty);
    }

    /// <summary>
    /// Sets the opacity of the default DragAdorner.
    /// </summary>
    public static void SetDefaultDragAdornerOpacity(UIElement target, double value)
    {
      target.SetValue(DefaultDragAdornerOpacityProperty, value);
    }

    /// <summary>
    /// Gets or Sets the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.
    /// </summary>
    public static readonly DependencyProperty DragMouseAnchorPointProperty
      = DependencyProperty.RegisterAttached("DragMouseAnchorPoint",
                                            typeof(Point),
                                            typeof(DragDrop),
                                            new PropertyMetadata(new Point(0, 1)));

    /// <summary>
    /// Gets the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.
    /// </summary>
    public static Point GetDragMouseAnchorPoint(UIElement target)
    {
      return (Point)target.GetValue(DragMouseAnchorPointProperty);
    }

    /// <summary>
    /// Sets the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.
    /// </summary>
    public static void SetDragMouseAnchorPoint(UIElement target, Point value)
    {
      target.SetValue(DragMouseAnchorPointProperty, value);
    }

    /// <summary>
    /// Gets or Sets a DataTemplate for the DragAdorner.
    /// </summary>
    public static readonly DependencyProperty DragAdornerTemplateProperty
      = DependencyProperty.RegisterAttached("DragAdornerTemplate",
                                            typeof(DataTemplate),
                                            typeof(DragDrop));

    /// <summary>
    /// Gets the DataTemplate for the DragAdorner.
    /// </summary>
    public static DataTemplate GetDragAdornerTemplate(UIElement target)
    {
      return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
    }

    /// <summary>
    /// Sets the DataTemplate for the DragAdorner.
    /// </summary>
    public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(DragAdornerTemplateProperty, value);
    }

    /// <summary>
    /// Gets or Sets a DataTemplateSelector for the DragAdorner.
    /// </summary>
    public static readonly DependencyProperty DragAdornerTemplateSelectorProperty
      = DependencyProperty.RegisterAttached("DragAdornerTemplateSelector",
                                            typeof(DataTemplateSelector),
                                            typeof(DragDrop),
                                            new PropertyMetadata(default(DataTemplateSelector)));

    /// <summary>
    /// Gets the DataTemplateSelector for the DragAdorner.
    /// </summary>
    public static void SetDragAdornerTemplateSelector(DependencyObject element, DataTemplateSelector value)
    {
      element.SetValue(DragAdornerTemplateSelectorProperty, value);
    }

    /// <summary>
    /// Gets the DataTemplateSelector for the DragAdorner.
    /// </summary>
    public static DataTemplateSelector GetDragAdornerTemplateSelector(DependencyObject element)
    {
      return (DataTemplateSelector)element.GetValue(DragAdornerTemplateSelectorProperty);
    }

    /// <summary>
    /// Use descendant bounds of the VisualSourceItem as MinWidth for the DragAdorner.
    /// </summary>
    public static readonly DependencyProperty UseVisualSourceItemSizeForDragAdornerProperty
      = DependencyProperty.RegisterAttached("UseVisualSourceItemSizeForDragAdorner",
                                            typeof(bool),
                                            typeof(DragDrop),
                                            new PropertyMetadata(false));

    /// <summary>
    /// Get the flag which indicates if the DragAdorner use the descendant bounds of the VisualSourceItem as MinWidth.
    /// </summary>
    public static bool GetUseVisualSourceItemSizeForDragAdorner(UIElement target)
    {
      return (bool)target.GetValue(UseVisualSourceItemSizeForDragAdornerProperty);
    }

    /// <summary>
    /// Set the flag which indicates if the DragAdorner use the descendant bounds of the VisualSourceItem as MinWidth.
    /// </summary>
    public static void SetUseVisualSourceItemSizeForDragAdorner(UIElement target, bool value)
    {
      target.SetValue(UseVisualSourceItemSizeForDragAdornerProperty, value);
    }

    /// <summary>
    /// Gets or Sets whether if the default DataTemplate for the effects should be use.
    /// </summary>
    public static readonly DependencyProperty UseDefaultEffectDataTemplateProperty
      = DependencyProperty.RegisterAttached("UseDefaultEffectDataTemplate",
                                            typeof(bool),
                                            typeof(DragDrop),
                                            new PropertyMetadata(false));

    /// <summary>
    /// Gets whether if the default DataTemplate for the effects should be use.
    /// </summary>
    public static bool GetUseDefaultEffectDataTemplate(UIElement target)
    {
      return (bool)target.GetValue(UseDefaultEffectDataTemplateProperty);
    }

    /// <summary>
    /// Sets whether if the default DataTemplate for the effects should be use.
    /// </summary>
    public static void SetUseDefaultEffectDataTemplate(UIElement target, bool value)
    {
      target.SetValue(UseDefaultEffectDataTemplateProperty, value);
    }

    /// <summary>
    /// Gets or Sets a EffectAdorner DataTemplate for effect type None.
    /// </summary>
    public static readonly DependencyProperty EffectNoneAdornerTemplateProperty
      = DependencyProperty.RegisterAttached("EffectNoneAdornerTemplate",
                                            typeof(DataTemplate),
                                            typeof(DragDrop),
                                            new PropertyMetadata((DataTemplate)null));

    /// <summary>
    /// Gets a EffectAdorner DataTemplate for effect type None.
    /// </summary>
    public static DataTemplate GetEffectNoneAdornerTemplate(UIElement target)
    {
      return (DataTemplate)target.GetValue(EffectNoneAdornerTemplateProperty);
    }

    /// <summary>
    /// Sets a EffectAdorner DataTemplate for effect type None.
    /// </summary>
    public static void SetEffectNoneAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(EffectNoneAdornerTemplateProperty, value);
    }

    /// <summary>
    /// Gets or Sets a EffectAdorner DataTemplate for effect type Copy.
    /// </summary>
    public static readonly DependencyProperty EffectCopyAdornerTemplateProperty
      = DependencyProperty.RegisterAttached("EffectCopyAdornerTemplate",
                                            typeof(DataTemplate),
                                            typeof(DragDrop),
                                            new PropertyMetadata((DataTemplate)null));

    /// <summary>
    /// Gets a EffectAdorner DataTemplate for effect type Copy.
    /// </summary>
    public static DataTemplate GetEffectCopyAdornerTemplate(UIElement target)
    {
      return (DataTemplate)target.GetValue(EffectCopyAdornerTemplateProperty);
    }

    /// <summary>
    /// Sets a EffectAdorner DataTemplate for effect type Copy.
    /// </summary>
    public static void SetEffectCopyAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(EffectCopyAdornerTemplateProperty, value);
    }

    /// <summary>
    /// Gets or Sets a EffectAdorner DataTemplate for effect type Move.
    /// </summary>
    public static readonly DependencyProperty EffectMoveAdornerTemplateProperty
      = DependencyProperty.RegisterAttached("EffectMoveAdornerTemplate",
                                            typeof(DataTemplate),
                                            typeof(DragDrop),
                                            new PropertyMetadata((DataTemplate)null));

    /// <summary>
    /// Gets a EffectAdorner DataTemplate for effect type Move.
    /// </summary>
    public static DataTemplate GetEffectMoveAdornerTemplate(UIElement target)
    {
      return (DataTemplate)target.GetValue(EffectMoveAdornerTemplateProperty);
    }

    /// <summary>
    /// Sets a EffectAdorner DataTemplate for effect type Move.
    /// </summary>
    public static void SetEffectMoveAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(EffectMoveAdornerTemplateProperty, value);
    }

    /// <summary>
    /// Gets or Sets a EffectAdorner DataTemplate for effect type Link.
    /// </summary>
    public static readonly DependencyProperty EffectLinkAdornerTemplateProperty
      = DependencyProperty.RegisterAttached("EffectLinkAdornerTemplate",
                                            typeof(DataTemplate),
                                            typeof(DragDrop),
                                            new PropertyMetadata((DataTemplate)null));

    /// <summary>
    /// Gets a EffectAdorner DataTemplate for effect type Link.
    /// </summary>
    public static DataTemplate GetEffectLinkAdornerTemplate(UIElement target)
    {
      return (DataTemplate)target.GetValue(EffectLinkAdornerTemplateProperty);
    }

    /// <summary>
    /// Sets a EffectAdorner DataTemplate for effect type Link.
    /// </summary>
    public static void SetEffectLinkAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(EffectLinkAdornerTemplateProperty, value);
    }

    /// <summary>
    /// Gets or Sets a EffectAdorner DataTemplate for effect type All.
    /// </summary>
    public static readonly DependencyProperty EffectAllAdornerTemplateProperty
      = DependencyProperty.RegisterAttached("EffectAllAdornerTemplate",
                                            typeof(DataTemplate),
                                            typeof(DragDrop),
                                            new PropertyMetadata((DataTemplate)null));

    /// <summary>
    /// Gets a EffectAdorner DataTemplate for effect type All.
    /// </summary>
    public static DataTemplate GetEffectAllAdornerTemplate(UIElement target)
    {
      return (DataTemplate)target.GetValue(EffectAllAdornerTemplateProperty);
    }

    /// <summary>
    /// Sets a EffectAdorner DataTemplate for effect type All.
    /// </summary>
    public static void SetEffectAllAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(EffectAllAdornerTemplateProperty, value);
    }

    /// <summary>
    /// Gets or Sets a EffectAdorner DataTemplate for effect type Scroll.
    /// </summary>
    public static readonly DependencyProperty EffectScrollAdornerTemplateProperty
      = DependencyProperty.RegisterAttached("EffectScrollAdornerTemplate",
                                            typeof(DataTemplate),
                                            typeof(DragDrop),
                                            new PropertyMetadata((DataTemplate)null));

    /// <summary>
    /// Gets a EffectAdorner DataTemplate for effect type Scroll.
    /// </summary>
    public static DataTemplate GetEffectScrollAdornerTemplate(UIElement target)
    {
      return (DataTemplate)target.GetValue(EffectScrollAdornerTemplateProperty);
    }

    /// <summary>
    /// Sets a EffectAdorner DataTemplate for effect type Scroll.
    /// </summary>
    public static void SetEffectScrollAdornerTemplate(UIElement target, DataTemplate value)
    {
      target.SetValue(EffectScrollAdornerTemplateProperty, value);
    }

    /// <summary>
    /// Gets or Sets the Orientation which should be used for the drag drop action (default null).
    /// Normally it will be look up to find the correct orientaion of the inner ItemsPanel,
    /// but sometimes it's necessary to force the oreintation, if the look up is wrong.
    /// </summary>
    public static readonly DependencyProperty ItemsPanelOrientationProperty
      = DependencyProperty.RegisterAttached("ItemsPanelOrientation",
                                            typeof(Orientation?),
                                            typeof(DragDrop),
                                            new PropertyMetadata(null));

    /// <summary>
    /// Gets the Orientation which should be used for the drag drop action (default null).
    /// Normally it will be look up to find the correct orientaion of the inner ItemsPanel,
    /// but sometimes it's necessary to force the oreintation, if the look up is wrong.
    /// </summary>
    public static Orientation? GetItemsPanelOrientation(UIElement source)
    {
      return (Orientation?)source.GetValue(ItemsPanelOrientationProperty);
    }

    /// <summary>
    /// Sets the Orientation which should be used for the drag drop action (default null).
    /// Normally it will be look up to find the correct orientaion of the inner ItemsPanel,
    /// but sometimes it's necessary to force the oreintation, if the look up is wrong.
    /// </summary>
    public static void SetItemsPanelOrientation(UIElement source, Orientation? value)
    {
      source.SetValue(ItemsPanelOrientationProperty, value);
    }
  }
}