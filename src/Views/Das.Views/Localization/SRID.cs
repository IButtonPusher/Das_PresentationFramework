using System;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Das.Views.Localization
{
    public static class SRID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetResourceString(String resourceKey,
                                                 String defaultValue = null)
        {
            return ResourceManager.GetString(resourceKey, Culture);
        }

        public static ResourceManager ResourceManager =>
            s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(SR)));

        public static CultureInfo Culture { get; set; }

        public const String AbsoluteUriNotAllowed = "AbsoluteUriNotAllowed";
        public const String AbsoluteUriOnly = "AbsoluteUriOnly";
        public const String AccessCollectionAfterShutDown = "AccessCollectionAfterShutDown";
        public const String AddAnnotationsNotImplemented = "AddAnnotationsNotImplemented";
        public const String AddedItemNotAtIndex = "AddedItemNotAtIndex";
        public const String AddedItemNotInCollection = "AddedItemNotInCollection";
        public const String AdornedElementNotFound = "AdornedElementNotFound";
        public const String AdornedElementPlaceholderMustBeInTemplate = "AdornedElementPlaceholderMustBeInTemplate";
        public const String AdornerNotFound = "AdornerNotFound";
        public const String AffectedByMsCtfIssue = "AffectedByMsCtfIssue";
        public const String AlreadyHasLogicalChildren = "AlreadyHasLogicalChildren";
        public const String AlreadyHasParent = "AlreadyHasParent";
        public const String AnnotationAdorner_NotUIElement = "AnnotationAdorner_NotUIElement";
        public const String AnnotationAlreadyExistInService = "AnnotationAlreadyExistInService";
        public const String AnnotationAlreadyExists = "AnnotationAlreadyExists";
        public const String AnnotationIsNull = "AnnotationIsNull";
        public const String AnnotationServiceAlreadyExists = "AnnotationServiceAlreadyExists";
        public const String AnnotationServiceIsAlreadyEnabled = "AnnotationServiceIsAlreadyEnabled";
        public const String AnnotationServiceNotEnabled = "AnnotationServiceNotEnabled";
        public const String AppActivationException = "AppActivationException";
        public const String ApplicationAlreadyRunning = "ApplicationAlreadyRunning";
        public const String ApplicationShuttingDown = "ApplicationShuttingDown";
        public const String ArgumentLengthMismatch = "ArgumentLengthMismatch";
        public const String ArgumentPropertyMustNotBeNull = "ArgumentPropertyMustNotBeNull";
        public const String Argument_InvalidOffLen = "Argument_InvalidOffLen";
        public const String ArrangeReentrancyInvalid = "ArrangeReentrancyInvalid";
        public const String AssemblyIdNegative = "AssemblyIdNegative";
        public const String AssemblyIdOutOfSequence = "AssemblyIdOutOfSequence";
        public const String AssemblyTagMissing = "AssemblyTagMissing";
        public const String AttachablePropertyNotFound = "AttachablePropertyNotFound";
        public const String AudioVideo_CannotControlMedia = "AudioVideo_CannotControlMedia";
        public const String AudioVideo_InvalidDependencyObject = "AudioVideo_InvalidDependencyObject";
        public const String AudioVideo_InvalidMediaState = "AudioVideo_InvalidMediaState";

        public const String AuxiliaryFilterReturnedAnomalousCountOfCharacters =
            "AuxiliaryFilterReturnedAnomalousCountOfCharacters";

        public const String AxNoConnectionPoint = "AxNoConnectionPoint";
        public const String AxNoConnectionPointContainer = "AxNoConnectionPointContainer";
        public const String AxNoEventInterface = "AxNoEventInterface";
        public const String AXNohWnd = "AXNohWnd";
        public const String AxNoSinkAdvise = "AxNoSinkAdvise";
        public const String AxNoSinkImplementation = "AxNoSinkImplementation";
        public const String AxRequiresApartmentThread = "AxRequiresApartmentThread";
        public const String AxWindowlessControl = "AxWindowlessControl";
        public const String BadDistance = "BadDistance";
        public const String BadFixedTextPosition = "BadFixedTextPosition";
        public const String BadTargetArray = "BadTargetArray";
        public const String BadTextPositionOrder = "BadTextPositionOrder";
        public const String BamlAssemblyIdNotFound = "BamlAssemblyIdNotFound";
        public const String BamlBadExtensionValue = "BamlBadExtensionValue";

        public const String BamlIsNotSupportedOutsideOfApplicationResources =
            "BamlIsNotSupportedOutsideOfApplicationResources";

        public const String BamlReaderClosed = "BamlReaderClosed";
        public const String BamlReaderNoOwnerType = "BamlReaderNoOwnerType";
        public const String BamlScopeError = "BamlScopeError";
        public const String BamlTypeIdNotFound = "BamlTypeIdNotFound";
        public const String BamlWriterBadAssembly = "BamlWriterBadAssembly";
        public const String BamlWriterBadScope = "BamlWriterBadScope";
        public const String BamlWriterBadStream = "BamlWriterBadStream";
        public const String BamlWriterBadXmlns = "BamlWriterBadXmlns";
        public const String BamlWriterClosed = "BamlWriterClosed";
        public const String BamlWriterNoInElement = "BamlWriterNoInElement";
        public const String BamlWriterStartDoc = "BamlWriterStartDoc";
        public const String BamlWriterUnknownMarkupExtension = "BamlWriterUnknownMarkupExtension";
        public const String BindingCollectionContainsNonBinding = "BindingCollectionContainsNonBinding";
        public const String BindingConflict = "BindingConflict";
        public const String BindingExpressionIsDetached = "BindingExpressionIsDetached";
        public const String BindingExpressionStatusChanged = "BindingExpressionStatusChanged";
        public const String BindingGroup_CannotChangeGroups = "BindingGroup_CannotChangeGroups";
        public const String BindingGroup_NoEntry = "BindingGroup_NoEntry";
        public const String BindingGroup_ValueUnavailable = "BindingGroup_ValueUnavailable";
        public const String BindingListCannotCustomFilter = "BindingListCannotCustomFilter";
        public const String BindingListCanOnlySortByOneProperty = "BindingListCanOnlySortByOneProperty";
        public const String BufferOffsetNegative = "BufferOffsetNegative";
        public const String BufferTooSmall = "BufferTooSmall";
        public const String ByteRangeDownloaderNotInitialized = "ByteRangeDownloaderNotInitialized";
        public const String CalendarAutomationPeer_BlackoutDayHelpText = "CalendarAutomationPeer_BlackoutDayHelpText";

        public const String CalendarAutomationPeer_CalendarButtonLocalizedControlType =
            "CalendarAutomationPeer_CalendarButtonLocalizedControlType";

        public const String CalendarAutomationPeer_DayButtonLocalizedControlType =
            "CalendarAutomationPeer_DayButtonLocalizedControlType";

        public const String CalendarAutomationPeer_DecadeMode = "CalendarAutomationPeer_DecadeMode";
        public const String CalendarAutomationPeer_MonthMode = "CalendarAutomationPeer_MonthMode";
        public const String CalendarAutomationPeer_YearMode = "CalendarAutomationPeer_YearMode";

        public const String CalendarCollection_MultiThreadedCollectionChangeNotSupported =
            "CalendarCollection_MultiThreadedCollectionChangeNotSupported";

        public const String CalendarNamePropertyValueNotValid = "CalendarNamePropertyValueNotValid";

        public const String Calendar_CheckSelectionMode_InvalidOperation =
            "Calendar_CheckSelectionMode_InvalidOperation";

        public const String Calendar_NextButtonName = "Calendar_NextButtonName";

        public const String Calendar_OnDisplayModePropertyChanged_InvalidValue =
            "Calendar_OnDisplayModePropertyChanged_InvalidValue";

        public const String Calendar_OnFirstDayOfWeekChanged_InvalidValue =
            "Calendar_OnFirstDayOfWeekChanged_InvalidValue";

        public const String Calendar_OnSelectedDateChanged_InvalidOperation =
            "Calendar_OnSelectedDateChanged_InvalidOperation";

        public const String Calendar_OnSelectedDateChanged_InvalidValue =
            "Calendar_OnSelectedDateChanged_InvalidValue";

        public const String Calendar_OnSelectionModeChanged_InvalidValue =
            "Calendar_OnSelectionModeChanged_InvalidValue";

        internal const string NotInInitialization = "NotInInitialization";
        internal const string InInitialization = "InInitialization";
        internal const string OnlyOneInitialization = "OnlyOneInitialization";

        public const String Calendar_PreviousButtonName = "Calendar_PreviousButtonName";
        public const String Calendar_UnSelectableDates = "Calendar_UnSelectableDates";
        public const String CancelEditNotSupported = "CancelEditNotSupported";
        public const String CancelledText = "CancelledText";
        public const String CancelledTitle = "CancelledTitle";
        public const String CannotBeInsidePopup = "CannotBeInsidePopup";
        public const String CannotBeSelfParent = "CannotBeSelfParent";
        public const String CannotCallRunFromBrowserHostedApp = "CannotCallRunFromBrowserHostedApp";
        public const String CannotCallRunMultipleTimes = "CannotCallRunMultipleTimes";
        public const String CannotChangeAfterSealed = "CannotChangeAfterSealed";
        public const String CannotChangeLiveShaping = "CannotChangeLiveShaping";
        public const String CannotChangeMainWindowInBrowser = "CannotChangeMainWindowInBrowser";

        public const String CannotDetermineSortByPropertiesForCollection =
            "CannotDetermineSortByPropertiesForCollection";

        public const String CannotEditPlaceholder = "CannotEditPlaceholder";
        public const String CannotFilterView = "CannotFilterView";
        public const String CannotFindRemovedItem = "CannotFindRemovedItem";
        public const String CannotGroupView = "CannotGroupView";
        public const String CannotHaveEventHandlersInThemeStyle = "CannotHaveEventHandlersInThemeStyle";

        public const String CannotHaveOverridesDefaultStyleInThemeStyle =
            "CannotHaveOverridesDefaultStyleInThemeStyle";

        public const String CannotHavePropertyInStyle = "CannotHavePropertyInStyle";
        public const String CannotHavePropertyInTemplate = "CannotHavePropertyInTemplate";
        public const String CannotHookupFCERoot = "CannotHookupFCERoot";
        public const String CannotInvokeScript = "CannotInvokeScript";
        public const String CannotModifyLogicalChildrenDuringTreeWalk = "CannotModifyLogicalChildrenDuringTreeWalk";
        public const String CannotMoveToUnknownPosition = "CannotMoveToUnknownPosition";
        public const String CannotParseId = "CannotParseId";
        public const String CannotProcessInkCommand = "CannotProcessInkCommand";

        public const String CannotQueryPropertiesWhenPageNotInTreeWithWindow =
            "CannotQueryPropertiesWhenPageNotInTreeWithWindow";

        public const String CannotRecyleHeterogeneousTypes = "CannotRecyleHeterogeneousTypes";
        public const String CannotRemoveUnrealizedItems = "CannotRemoveUnrealizedItems";
        public const String CannotSelectNotSelectableItem = "CannotSelectNotSelectableItem";
        public const String CannotSerializeInvalidInstance = "CannotSerializeInvalidInstance";
        public const String CannotSetNegativePosition = "CannotSetNegativePosition";
        public const String CannotSetOwnerToItself = "CannotSetOwnerToItself";
        public const String CannotSortView = "CannotSortView";
        public const String CannotUseItemsSource = "CannotUseItemsSource";
        public const String CannotWriteToReadOnly = "CannotWriteToReadOnly";
        public const String CanOnlyHaveOneChild = "CanOnlyHaveOneChild";
        public const String CantSetInMarkup = "CantSetInMarkup";
        public const String CantSetOwnerAfterDialogIsShown = "CantSetOwnerAfterDialogIsShown";
        public const String CantSetOwnerToClosedWindow = "CantSetOwnerToClosedWindow";
        public const String CantSetOwnerWhosHwndIsNotCreated = "CantSetOwnerWhosHwndIsNotCreated";
        public const String CantShowMBServiceWithOwner = "CantShowMBServiceWithOwner";
        public const String CantShowModalOnNonInteractive = "CantShowModalOnNonInteractive";
        public const String CantShowOnDifferentThread = "CantShowOnDifferentThread";
        public const String CantSwitchVirtualizationModePostMeasure = "CantSwitchVirtualizationModePostMeasure";
        public const String ChangeNotAllowedAfterShow = "ChangeNotAllowedAfterShow";
        public const String ChangeSealedBinding = "ChangeSealedBinding";
        public const String ChangingCollectionNotSupported = "ChangingCollectionNotSupported";
        public const String ChangingIdNotAllowed = "ChangingIdNotAllowed";
        public const String ChangingTypeNotAllowed = "ChangingTypeNotAllowed";
        public const String ChildHasWrongType = "ChildHasWrongType";
        public const String ChildNameMustBeNonEmpty = "ChildNameMustBeNonEmpty";
        public const String ChildNameNamePatternReserved = "ChildNameNamePatternReserved";
        public const String ChildTemplateInstanceDoesNotExist = "ChildTemplateInstanceDoesNotExist";
        public const String ChildWindowMustHaveCorrectParent = "ChildWindowMustHaveCorrectParent";
        public const String ChildWindowNotCreated = "ChildWindowNotCreated";
        public const String CircularOwnerChild = "CircularOwnerChild";
        public const String ClearHighlight = "ClearHighlight";
        public const String ClipboardCopyMode_Disabled = "ClipboardCopyMode_Disabled";
        public const String ClipToBoundsNotSupported = "ClipToBoundsNotSupported";
        public const String CollectionAddEventMissingItem = "CollectionAddEventMissingItem";
        public const String CollectionChangeIndexOutOfRange = "CollectionChangeIndexOutOfRange";

        public const String CollectionContainerMustBeUniqueForComposite =
            "CollectionContainerMustBeUniqueForComposite";

        public const String CollectionViewTypeIsInitOnly = "CollectionViewTypeIsInitOnly";

        public const String CollectionView_MissingSynchronizationCallback =
            "CollectionView_MissingSynchronizationCallback";

        public const String CollectionView_NameTypeDuplicity = "CollectionView_NameTypeDuplicity";
        public const String CollectionView_ViewTypeInsufficient = "CollectionView_ViewTypeInsufficient";
        public const String CollectionView_WrongType = "CollectionView_WrongType";
        public const String Collection_NoNull = "Collection_NoNull";
        public const String ColorConvertedBitmapExtensionNoSourceImage = "ColorConvertedBitmapExtensionNoSourceImage";

        public const String ColorConvertedBitmapExtensionNoSourceProfile =
            "ColorConvertedBitmapExtensionNoSourceProfile";

        public const String ColorConvertedBitmapExtensionSyntax = "ColorConvertedBitmapExtensionSyntax";
        public const String CompatibilityPreferencesSealed = "CompatibilityPreferencesSealed";
        public const String ComponentAlreadyInPresentationContext = "ComponentAlreadyInPresentationContext";
        public const String ComponentNotInPresentationContext = "ComponentNotInPresentationContext";
        public const String CompositeCollectionResetOnlyOnClear = "CompositeCollectionResetOnlyOnClear";
        public const String ConditionCannotUseBothPropertyAndBinding = "ConditionCannotUseBothPropertyAndBinding";
        public const String ConditionValueOfExpressionNotSupported = "ConditionValueOfExpressionNotSupported";

        public const String ConditionValueOfMarkupExtensionNotSupported =
            "ConditionValueOfMarkupExtensionNotSupported";

        public const String ContentControlCannotHaveMultipleContent = "ContentControlCannotHaveMultipleContent";
        public const String ContentTypeNotSupported = "ContentTypeNotSupported";
        public const String ContextMenuInDifferentDispatcher = "ContextMenuInDifferentDispatcher";
        public const String CopyToNotEnoughSpace = "CopyToNotEnoughSpace";

        public const String CorePropertyEnumeratorPositionedOutOfBounds =
            "CorePropertyEnumeratorPositionedOutOfBounds";

        public const String CreateHighlight = "CreateHighlight";
        public const String CreateInkNote = "CreateInkNote";
        public const String CreateRootPopup_ChildHasLogicalParent = "CreateRootPopup_ChildHasLogicalParent";
        public const String CreateRootPopup_ChildHasVisualParent = "CreateRootPopup_ChildHasVisualParent";
        public const String CreateTextNote = "CreateTextNote";
        public const String CrossThreadAccessOfUnshareableFreezable = "CrossThreadAccessOfUnshareableFreezable";
        public const String CustomContentStateMustBeSerializable = "CustomContentStateMustBeSerializable";
        public const String CustomDictionaryFailedToLoadDictionaryUri = "CustomDictionaryFailedToLoadDictionaryUri";

        public const String CustomDictionaryFailedToUnLoadDictionaryUri =
            "CustomDictionaryFailedToUnLoadDictionaryUri";

        public const String CustomDictionaryItemAlreadyExists = "CustomDictionaryItemAlreadyExists";
        public const String CustomDictionaryNullItem = "CustomDictionaryNullItem";
        public const String CustomDictionarySourcesUnsupportedURI = "CustomDictionarySourcesUnsupportedURI";
        public const String CyclicStyleReferenceDetected = "CyclicStyleReferenceDetected";
        public const String CyclicThemeStyleReferenceDetected = "CyclicThemeStyleReferenceDetected";

        public const String DataGridCellItemAutomationPeer_LocalizedControlType =
            "DataGridCellItemAutomationPeer_LocalizedControlType";

        public const String DataGridCellItemAutomationPeer_NameCoreFormat =
            "DataGridCellItemAutomationPeer_NameCoreFormat";

        public const String DataGridColumnHeaderItemAutomationPeer_NameCoreFormat =
            "DataGridColumnHeaderItemAutomationPeer_NameCoreFormat";

        public const String DataGridColumnHeaderItemAutomationPeer_Unresizable =
            "DataGridColumnHeaderItemAutomationPeer_Unresizable";

        public const String DataGridColumnHeaderItemAutomationPeer_Unsupported =
            "DataGridColumnHeaderItemAutomationPeer_Unsupported";

        public const String DataGridLength_Infinity = "DataGridLength_Infinity";
        public const String DataGridLength_InvalidType = "DataGridLength_InvalidType";
        public const String DataGridRow_CannotSelectRowWhenCells = "DataGridRow_CannotSelectRowWhenCells";
        public const String DataGrid_AutomationInvokeFailed = "DataGrid_AutomationInvokeFailed";
        public const String DataGrid_CannotSelectCell = "DataGrid_CannotSelectCell";
        public const String DataGrid_ColumnDisplayIndexOutOfRange = "DataGrid_ColumnDisplayIndexOutOfRange";
        public const String DataGrid_ColumnIndexOutOfRange = "DataGrid_ColumnIndexOutOfRange";
        public const String DataGrid_ColumnIsReadOnly = "DataGrid_ColumnIsReadOnly";
        public const String DataGrid_DisplayIndexOutOfRange = "DataGrid_DisplayIndexOutOfRange";
        public const String DataGrid_DuplicateDisplayIndex = "DataGrid_DuplicateDisplayIndex";
        public const String DataGrid_InvalidColumnReuse = "DataGrid_InvalidColumnReuse";
        public const String DataGrid_InvalidSortDescription = "DataGrid_InvalidSortDescription";
        public const String DataGrid_NewColumnInvalidDisplayIndex = "DataGrid_NewColumnInvalidDisplayIndex";
        public const String DataGrid_NullColumn = "DataGrid_NullColumn";
        public const String DataGrid_ProbableInvalidSortDescription = "DataGrid_ProbableInvalidSortDescription";
        public const String DataGrid_ReadonlyCellsItemsSource = "DataGrid_ReadonlyCellsItemsSource";
        public const String DataTypeCannotBeObject = "DataTypeCannotBeObject";

        public const String DatePickerAutomationPeer_LocalizedControlType =
            "DatePickerAutomationPeer_LocalizedControlType";

        public const String DatePickerTextBox_DefaultWatermarkText = "DatePickerTextBox_DefaultWatermarkText";

        public const String DatePickerTextBox_TemplatePartIsOfIncorrectType =
            "DatePickerTextBox_TemplatePartIsOfIncorrectType";

        public const String DatePicker_DropDownButtonName = "DatePicker_DropDownButtonName";

        public const String DatePicker_OnSelectedDateFormatChanged_InvalidValue =
            "DatePicker_OnSelectedDateFormatChanged_InvalidValue";

        public const String DatePicker_WatermarkText = "DatePicker_WatermarkText";
        public const String DeferringLoaderNoContext = "DeferringLoaderNoContext";
        public const String DeferringLoaderNoSave = "DeferringLoaderNoSave";
        public const String DeferSelectionActive = "DeferSelectionActive";
        public const String DeferSelectionNotActive = "DeferSelectionNotActive";
        public const String DeleteAnnotations = "DeleteAnnotations";
        public const String DeleteNotes = "DeleteNotes";
        public const String DeployText = "DeployText";
        public const String DeployTitle = "DeployTitle";

        public const String DesignerMetadata_CustomCategory_Accessibility =
            "DesignerMetadata_CustomCategory_Accessibility";

        public const String DesignerMetadata_CustomCategory_Content = "DesignerMetadata_CustomCategory_Content";
        public const String DesignerMetadata_CustomCategory_Navigation = "DesignerMetadata_CustomCategory_Navigation";
        public const String DialogResultMustBeSetAfterShowDialog = "DialogResultMustBeSetAfterShowDialog";
        public const String DisplayMemberPathAndItemTemplateDefined = "DisplayMemberPathAndItemTemplateDefined";

        public const String DisplayMemberPathAndItemTemplateSelectorDefined =
            "DisplayMemberPathAndItemTemplateSelectorDefined";

        public const String DocumentApplicationCannotInitializeUI = "DocumentApplicationCannotInitializeUI";

        public const String DocumentApplicationContextMenuFirstPageInputGesture =
            "DocumentApplicationContextMenuFirstPageInputGesture";

        public const String DocumentApplicationContextMenuLastPageInputGesture =
            "DocumentApplicationContextMenuLastPageInputGesture";

        public const String DocumentApplicationContextMenuNextPageHeader =
            "DocumentApplicationContextMenuNextPageHeader";

        public const String DocumentApplicationContextMenuNextPageInputGesture =
            "DocumentApplicationContextMenuNextPageInputGesture";

        public const String DocumentApplicationContextMenuPreviousPageHeader =
            "DocumentApplicationContextMenuPreviousPageHeader";

        public const String DocumentApplicationContextMenuPreviousPageInputGesture =
            "DocumentApplicationContextMenuPreviousPageInputGesture";

        public const String DocumentApplicationNotInFullTrust = "DocumentApplicationNotInFullTrust";
        public const String DocumentApplicationRegistryKeyNotFound = "DocumentApplicationRegistryKeyNotFound";
        public const String DocumentApplicationStatusLoaded = "DocumentApplicationStatusLoaded";
        public const String DocumentApplicationUnableToOpenDocument = "DocumentApplicationUnableToOpenDocument";
        public const String DocumentApplicationUnknownFileFormat = "DocumentApplicationUnknownFileFormat";
        public const String DocumentGridInvalidViewMode = "DocumentGridInvalidViewMode";

        public const String DocumentGridVisualTreeContainsNonBorderAsFirstElement =
            "DocumentGridVisualTreeContainsNonBorderAsFirstElement";

        public const String DocumentGridVisualTreeContainsNonDocumentGridPage =
            "DocumentGridVisualTreeContainsNonDocumentGridPage";

        public const String DocumentGridVisualTreeContainsNonUIElement = "DocumentGridVisualTreeContainsNonUIElement";
        public const String DocumentGridVisualTreeOutOfSync = "DocumentGridVisualTreeOutOfSync";
        public const String DocumentPageView_ParentNotDocumentPageHost = "DocumentPageView_ParentNotDocumentPageHost";
        public const String DocumentReadOnly = "DocumentReadOnly";
        public const String DocumentReferenceHasInvalidDocument = "DocumentReferenceHasInvalidDocument";
        public const String DocumentReferenceNotFound = "DocumentReferenceNotFound";
        public const String DocumentReferenceUnsupportedMimeType = "DocumentReferenceUnsupportedMimeType";
        public const String DocumentStructureUnexpectedParameterType1 = "DocumentStructureUnexpectedParameterType1";
        public const String DocumentStructureUnexpectedParameterType2 = "DocumentStructureUnexpectedParameterType2";
        public const String DocumentStructureUnexpectedParameterType4 = "DocumentStructureUnexpectedParameterType4";
        public const String DocumentStructureUnexpectedParameterType6 = "DocumentStructureUnexpectedParameterType6";
        public const String DocumentViewerArgumentMustBeInteger = "DocumentViewerArgumentMustBeInteger";
        public const String DocumentViewerArgumentMustBePercentage = "DocumentViewerArgumentMustBePercentage";
        public const String DocumentViewerCanHaveOnlyOneChild = "DocumentViewerCanHaveOnlyOneChild";

        public const String DocumentViewerChildMustImplementIDocumentPaginatorSource =
            "DocumentViewerChildMustImplementIDocumentPaginatorSource";

        public const String DocumentViewerOneMasterPage = "DocumentViewerOneMasterPage";

        public const String DocumentViewerOnlySupportsFixedDocumentSequence =
            "DocumentViewerOnlySupportsFixedDocumentSequence";

        public const String DocumentViewerPageViewsCollectionEmpty = "DocumentViewerPageViewsCollectionEmpty";
        public const String DocumentViewerSearchCompleteTitle = "DocumentViewerSearchCompleteTitle";
        public const String DocumentViewerSearchDownCompleteLabel = "DocumentViewerSearchDownCompleteLabel";
        public const String DocumentViewerSearchUpCompleteLabel = "DocumentViewerSearchUpCompleteLabel";
        public const String DocumentViewerStyleMustIncludeContentHost = "DocumentViewerStyleMustIncludeContentHost";
        public const String DocumentViewerViewFitToHeightCommandText = "DocumentViewerViewFitToHeightCommandText";

        public const String DocumentViewerViewFitToMaxPagesAcrossCommandText =
            "DocumentViewerViewFitToMaxPagesAcrossCommandText";

        public const String DocumentViewerViewFitToWidthCommandText = "DocumentViewerViewFitToWidthCommandText";
        public const String DocumentViewerViewThumbnailsCommandText = "DocumentViewerViewThumbnailsCommandText";
        public const String DownloadText = "DownloadText";
        public const String DownloadTitle = "DownloadTitle";
        public const String DragMoveFail = "DragMoveFail";
        public const String DuplicatedCompatibleUri = "DuplicatedCompatibleUri";
        public const String DuplicatedUri = "DuplicatedUri";
        public const String DuplicatesNotAllowed = "DuplicatesNotAllowed";
        public const String ElementMustBeInPopup = "ElementMustBeInPopup";
        public const String ElementMustBelongToTemplate = "ElementMustBelongToTemplate";
        public const String EmptySelectionNotSupported = "EmptySelectionNotSupported";
        public const String EndInitWithoutBeginInitNotSupported = "EndInitWithoutBeginInitNotSupported";
        public const String EntryAssemblyIsNull = "EntryAssemblyIsNull";
        public const String EnumeratorCollectionDisposed = "EnumeratorCollectionDisposed";
        public const String EnumeratorInvalidOperation = "EnumeratorInvalidOperation";
        public const String EnumeratorNotStarted = "EnumeratorNotStarted";
        public const String EnumeratorReachedEnd = "EnumeratorReachedEnd";
        public const String EnumeratorVersionChanged = "EnumeratorVersionChanged";
        public const String EventTriggerBadAction = "EventTriggerBadAction";
        public const String EventTriggerDoesNotEnterExit = "EventTriggerDoesNotEnterExit";
        public const String EventTriggerDoNotSetProperties = "EventTriggerDoNotSetProperties";
        public const String EventTriggerEventUnresolvable = "EventTriggerEventUnresolvable";
        public const String EventTriggerNeedEvent = "EventTriggerNeedEvent";
        public const String EventTriggerOnStyleNotAllowedToHaveTarget = "EventTriggerOnStyleNotAllowedToHaveTarget";
        public const String EventTriggerTargetNameUnresolvable = "EventTriggerTargetNameUnresolvable";
        public const String ExceptionInGetPage = "ExceptionInGetPage";

        public const String ExceptionValidationRuleValidateNotSupported =
            "ExceptionValidationRuleValidateNotSupported";

        public const String ExpectedBamlSchemaContext = "ExpectedBamlSchemaContext";
        public const String ExpectedBinaryContent = "ExpectedBinaryContent";
        public const String ExpectedResourceDictionaryTarget = "ExpectedResourceDictionaryTarget";
        public const String FailedResumePageFunction = "FailedResumePageFunction";
        public const String FailedToConvertResource = "FailedToConvertResource";
        public const String FailToLaunchDefaultBrowser = "FailToLaunchDefaultBrowser";
        public const String FailToNavigateUsingHyperlinkTarget = "FailToNavigateUsingHyperlinkTarget";
        public const String FileDialogBufferTooSmall = "FileDialogBufferTooSmall";
        public const String FileDialogCreatePrompt = "FileDialogCreatePrompt";
        public const String FileDialogFileNotFound = "FileDialogFileNotFound";
        public const String FileDialogInvalidFileName = "FileDialogInvalidFileName";
        public const String FileDialogInvalidFilter = "FileDialogInvalidFilter";
        public const String FileDialogInvalidFilterIndex = "FileDialogInvalidFilterIndex";
        public const String FileDialogOverwritePrompt = "FileDialogOverwritePrompt";
        public const String FileDialogSubClassFailure = "FileDialogSubClassFailure";
        public const String FileNameMustNotBeNull = "FileNameMustNotBeNull";
        public const String FileNameNullOrEmpty = "FileNameNullOrEmpty";
        public const String FileToFilterNotLoaded = "FileToFilterNotLoaded";
        public const String FilterBindRegionNotImplemented = "FilterBindRegionNotImplemented";
        public const String FilterEndOfChunks = "FilterEndOfChunks";
        public const String FilterGetChunkNoStream = "FilterGetChunkNoStream";
        public const String FilterGetTextBufferOverflow = "FilterGetTextBufferOverflow";
        public const String FilterGetTextNotSupported = "FilterGetTextNotSupported";
        public const String FilterGetValueAlreadyCalledOnCurrentChunk = "FilterGetValueAlreadyCalledOnCurrentChunk";
        public const String FilterGetValueMustBeStringOrDateTime = "FilterGetValueMustBeStringOrDateTime";
        public const String FilterGetValueNotSupported = "FilterGetValueNotSupported";
        public const String FilterInitInvalidAttributes = "FilterInitInvalidAttributes";
        public const String FilterIPersistFileIsReadOnly = "FilterIPersistFileIsReadOnly";
        public const String FilterIPersistStreamIsReadOnly = "FilterIPersistStreamIsReadOnly";
        public const String FilterLoadInvalidModeFlag = "FilterLoadInvalidModeFlag";
        public const String FilterNullGetTextBufferPointer = "FilterNullGetTextBufferPointer";
        public const String FilterPropSpecUnknownUnionSelector = "FilterPropSpecUnknownUnionSelector";
        public const String FixedDocumentExpectsDependencyObject = "FixedDocumentExpectsDependencyObject";
        public const String FixedDocumentReadonly = "FixedDocumentReadonly";
        public const String FlowDocumentFormattingReentrancy = "FlowDocumentFormattingReentrancy";
        public const String FlowDocumentInvalidContnetChange = "FlowDocumentInvalidContnetChange";

        public const String FlowDocumentPageViewerOnlySupportsFlowDocument =
            "FlowDocumentPageViewerOnlySupportsFlowDocument";

        public const String FlowDocumentReaderCanHaveOnlyOneChild = "FlowDocumentReaderCanHaveOnlyOneChild";

        public const String FlowDocumentReaderCannotDisableAllViewingModes =
            "FlowDocumentReaderCannotDisableAllViewingModes";

        public const String FlowDocumentReaderDecoratorMarkedAsContentHostMustHaveNoContent =
            "FlowDocumentReaderDecoratorMarkedAsContentHostMustHaveNoContent";

        public const String FlowDocumentReaderViewingModeEnabledConflict =
            "FlowDocumentReaderViewingModeEnabledConflict";

        public const String FlowDocumentReader_MultipleViewProvider_PageViewName =
            "FlowDocumentReader_MultipleViewProvider_PageViewName";

        public const String FlowDocumentReader_MultipleViewProvider_ScrollViewName =
            "FlowDocumentReader_MultipleViewProvider_ScrollViewName";

        public const String FlowDocumentReader_MultipleViewProvider_TwoPageViewName =
            "FlowDocumentReader_MultipleViewProvider_TwoPageViewName";

        public const String FlowDocumentScrollViewerCanHaveOnlyOneChild =
            "FlowDocumentScrollViewerCanHaveOnlyOneChild";

        public const String FlowDocumentScrollViewerDocumentBelongsToAnotherFlowDocumentScrollViewerAlready =
            "FlowDocumentScrollViewerDocumentBelongsToAnotherFlowDocumentScrollViewerAlready";

        public const String FlowDocumentScrollViewerMarkedAsContentHostMustHaveNoContent =
            "FlowDocumentScrollViewerMarkedAsContentHostMustHaveNoContent";

        public const String FormatRestrictionsExceeded = "FormatRestrictionsExceeded";
        public const String FrameNoAddChild = "FrameNoAddChild";
        public const String FrameworkElementFactoryAlreadyParented = "FrameworkElementFactoryAlreadyParented";
        public const String FrameworkElementFactoryCannotAddText = "FrameworkElementFactoryCannotAddText";
        public const String FrameworkElementFactoryMustBeSealed = "FrameworkElementFactoryMustBeSealed";
        public const String GenerationInProgress = "GenerationInProgress";
        public const String GenerationNotInProgress = "GenerationNotInProgress";
        public const String Generator_CountIsWrong = "Generator_CountIsWrong";
        public const String Generator_Inconsistent = "Generator_Inconsistent";
        public const String Generator_ItemIsWrong = "Generator_ItemIsWrong";
        public const String Generator_MoreErrors = "Generator_MoreErrors";
        public const String Generator_Readme0 = "Generator_Readme0";
        public const String Generator_Readme1 = "Generator_Readme1";
        public const String Generator_Readme2 = "Generator_Readme2";
        public const String Generator_Readme3 = "Generator_Readme3";
        public const String Generator_Readme4 = "Generator_Readme4";
        public const String Generator_Readme5 = "Generator_Readme5";
        public const String Generator_Readme6 = "Generator_Readme6";
        public const String Generator_Readme7 = "Generator_Readme7";
        public const String Generator_Readme8 = "Generator_Readme8";
        public const String Generator_Readme9 = "Generator_Readme9";
        public const String Generator_Unnamed = "Generator_Unnamed";
        public const String GetResponseFailed = "GetResponseFailed";
        public const String GetStreamFailed = "GetStreamFailed";
        public const String GlyphsAdvanceWidthCannotBeNegative = "GlyphsAdvanceWidthCannotBeNegative";
        public const String GlyphsCaretStopsContainsHexDigits = "GlyphsCaretStopsContainsHexDigits";

        public const String GlyphsCaretStopsLengthCorrespondsToUnicodeString =
            "GlyphsCaretStopsLengthCorrespondsToUnicodeString";

        public const String GlyphsClusterBadCharactersBeforeBracket = "GlyphsClusterBadCharactersBeforeBracket";
        public const String GlyphsClusterMisplacedSeparator = "GlyphsClusterMisplacedSeparator";
        public const String GlyphsClusterNoMatchingBracket = "GlyphsClusterNoMatchingBracket";
        public const String GlyphsClusterNoNestedClusters = "GlyphsClusterNoNestedClusters";
        public const String GlyphsIndexRequiredIfNoUnicode = "GlyphsIndexRequiredIfNoUnicode";
        public const String GlyphsIndexRequiredWithinCluster = "GlyphsIndexRequiredWithinCluster";
        public const String GlyphsTooManyCommas = "GlyphsTooManyCommas";

        public const String GlyphsUnicodeStringAndIndicesCannotBothBeEmpty =
            "GlyphsUnicodeStringAndIndicesCannotBothBeEmpty";

        public const String GlyphsUnicodeStringIsTooShort = "GlyphsUnicodeStringIsTooShort";
        public const String GridCollection_CannotModifyReadOnly = "GridCollection_CannotModifyReadOnly";
        public const String GridCollection_DestArrayInvalidLength = "GridCollection_DestArrayInvalidLength";
        public const String GridCollection_DestArrayInvalidLowerBound = "GridCollection_DestArrayInvalidLowerBound";
        public const String GridCollection_DestArrayInvalidRank = "GridCollection_DestArrayInvalidRank";
        public const String GridCollection_InOtherCollection = "GridCollection_InOtherCollection";
        public const String GridCollection_MustBeCertainType = "GridCollection_MustBeCertainType";
        public const String Grid_UnexpectedParameterType = "Grid_UnexpectedParameterType";
        public const String HandlerTypeIllegal = "HandlerTypeIllegal";
        public const String HasLogicalParent = "HasLogicalParent";
        public const String HostedWindowMustBeAChildWindow = "HostedWindowMustBeAChildWindow";
        public const String HostingStatusCancelled = "HostingStatusCancelled";
        public const String HostingStatusDownloadApp = "HostingStatusDownloadApp";
        public const String HostingStatusDownloadAppInfo = "HostingStatusDownloadAppInfo";
        public const String HostingStatusFailed = "HostingStatusFailed";
        public const String HostingStatusPreparingToRun = "HostingStatusPreparingToRun";
        public const String HostingStatusVerifying = "HostingStatusVerifying";
        public const String HwndHostDoesNotSupportChildKeyboardSinks = "HwndHostDoesNotSupportChildKeyboardSinks";
        public const String HyperLinkTargetNotFound = "HyperLinkTargetNotFound";
        public const String HyphenatorDisposed = "HyphenatorDisposed";
        public const String IconMustBeBitmapFrame = "IconMustBeBitmapFrame";
        public const String IDPInvalidContentPosition = "IDPInvalidContentPosition";
        public const String IDPNegativePageNumber = "IDPNegativePageNumber";
        public const String IllegalTreeChangeDetected = "IllegalTreeChangeDetected";
        public const String IllegalTreeChangeDetectedPostAction = "IllegalTreeChangeDetectedPostAction";
        public const String Illegal_InheritanceBehaviorSettor = "Illegal_InheritanceBehaviorSettor";
        public const String ImplementOtherMembersWithSort = "ImplementOtherMembersWithSort";
        public const String InavalidStartItem = "InavalidStartItem";
        public const String IncompatibleCLRText = "IncompatibleCLRText";
        public const String IncompatibleWinFXText = "IncompatibleWinFXText";
        public const String InconsistentBindingList = "InconsistentBindingList";
        public const String IncorrectAnchorLength = "IncorrectAnchorLength";
        public const String IncorrectFlowDirection = "IncorrectFlowDirection";
        public const String IncorrectLocatorPartType = "IncorrectLocatorPartType";
        public const String IndexedPropDescNotImplemented = "IndexedPropDescNotImplemented";
        public const String InDifferentParagraphs = "InDifferentParagraphs";
        public const String InDifferentScope = "InDifferentScope";
        public const String InDifferentTextContainers = "InDifferentTextContainers";
        public const String InkCanvasDeselectKeyDisplayString = "InkCanvasDeselectKeyDisplayString";
        public const String InputScopeAttribute_E_OUTOFMEMORY = "InputScopeAttribute_E_OUTOFMEMORY";
        public const String InputStreamMustBeReadable = "InputStreamMustBeReadable";
        public const String InsertInDeferSelectionActive = "InsertInDeferSelectionActive";
        public const String IntegerCollectionLengthLessThanZero = "IntegerCollectionLengthLessThanZero";
        public const String InvalidAnchorPosition = "InvalidAnchorPosition";
        public const String InvalidAttachedAnchor = "InvalidAttachedAnchor";
        public const String InvalidAttachedAnnotation = "InvalidAttachedAnnotation";
        public const String InvalidAttributeValue = "InvalidAttributeValue";
        public const String InvalidByteRanges = "InvalidByteRanges";
        public const String InvalidClipboardFormat = "InvalidClipboardFormat";
        public const String InvalidCompositionTarget = "InvalidCompositionTarget";
        public const String InvalidControlTemplateTargetType = "InvalidControlTemplateTargetType";
        public const String InvalidCtorParameterNoInfinity = "InvalidCtorParameterNoInfinity";
        public const String InvalidCtorParameterNoNaN = "InvalidCtorParameterNoNaN";
        public const String InvalidCtorParameterNoNegative = "InvalidCtorParameterNoNegative";
        public const String InvalidCtorParameterUnknownFigureUnitType = "InvalidCtorParameterUnknownFigureUnitType";
        public const String InvalidCtorParameterUnknownGridUnitType = "InvalidCtorParameterUnknownGridUnitType";

        public const String InvalidCtorParameterUnknownVirtualizationCacheLengthUnitType =
            "InvalidCtorParameterUnknownVirtualizationCacheLengthUnitType";

        public const String InvalidCustomSerialize = "InvalidCustomSerialize";
        public const String InvalidDeployText = "InvalidDeployText";
        public const String InvalidDeployTitle = "InvalidDeployTitle";
        public const String InvalidDeSerialize = "InvalidDeSerialize";
        public const String InvalidDiameter = "InvalidDiameter";
        public const String InvalidDSContentType = "InvalidDSContentType";
        public const String InvalidEmptyArray = "InvalidEmptyArray";
        public const String InvalidEmptyStrokeCollection = "InvalidEmptyStrokeCollection";
        public const String InvalidEndOfBaml = "InvalidEndOfBaml";
        public const String InvalidEventHandle = "InvalidEventHandle";
        public const String InvalidGuid = "InvalidGuid";
        public const String InvalidHighlightColor = "InvalidHighlightColor";
        public const String InvalidInkForeground = "InvalidInkForeground";
        public const String InvalidItemContainer = "InvalidItemContainer";
        public const String InvalidLocalizabilityValue = "InvalidLocalizabilityValue";
        public const String InvalidLocatorPart = "InvalidLocatorPart";
        public const String InvalidLocCommentTarget = "InvalidLocCommentTarget";
        public const String InvalidLocCommentValue = "InvalidLocCommentValue";
        public const String InvalidNamespace = "InvalidNamespace";
        public const String InvalidOperationDuringClosing = "InvalidOperationDuringClosing";
        public const String InvalidOperation_AddBackEntryNoContent = "InvalidOperation_AddBackEntryNoContent";
        public const String InvalidOperation_CannotClearFwdStack = "InvalidOperation_CannotClearFwdStack";
        public const String InvalidOperation_CannotReenterPageFunction = "InvalidOperation_CannotReenterPageFunction";

        public const String InvalidOperation_CantChangeJournalOwnership =
            "InvalidOperation_CantChangeJournalOwnership";

        public const String InvalidOperation_IComparerFailed = "InvalidOperation_IComparerFailed";

        public const String InvalidOperation_MustImplementIPCCSOrHandleNavigating =
            "InvalidOperation_MustImplementIPCCSOrHandleNavigating";

        public const String InvalidOperation_NoJournal = "InvalidOperation_NoJournal";
        public const String InvalidPageFunctionType = "InvalidPageFunctionType";
        public const String InvalidPoint = "InvalidPoint";
        public const String InvalidPropertyValue = "InvalidPropertyValue";
        public const String InvalidSelectionPages = "InvalidSelectionPages";
        public const String InvalidSetterValue = "InvalidSetterValue";
        public const String InvalidSFContentType = "InvalidSFContentType";
        public const String InvalidStartNodeForTextSelection = "InvalidStartNodeForTextSelection";
        public const String InvalidStartOfBaml = "InvalidStartOfBaml";
        public const String InvalidStickyNoteAnnotation = "InvalidStickyNoteAnnotation";
        public const String InvalidStickyNoteTemplate = "InvalidStickyNoteTemplate";
        public const String InvalidStoryFragmentsMarkup = "InvalidStoryFragmentsMarkup";
        public const String InvalidStringCornerRadius = "InvalidStringCornerRadius";
        public const String InvalidStringThickness = "InvalidStringThickness";
        public const String InvalidStringVirtualizationCacheLength = "InvalidStringVirtualizationCacheLength";
        public const String InvalidSubTreeProcessor = "InvalidSubTreeProcessor";
        public const String InvalidTempFileName = "InvalidTempFileName";
        public const String InvalidValueForTopLeft = "InvalidValueForTopLeft";
        public const String InvalidValueSpecified = "InvalidValueSpecified";
        public const String InvalidXmlContent = "InvalidXmlContent";
        public const String ItemCollectionHasNoCollection = "ItemCollectionHasNoCollection";
        public const String ItemCollectionRemoveArgumentOutOfRange = "ItemCollectionRemoveArgumentOutOfRange";
        public const String ItemCollectionShouldUseInnerSyncRoot = "ItemCollectionShouldUseInnerSyncRoot";
        public const String ItemsControl_ParentNotFrameworkElement = "ItemsControl_ParentNotFrameworkElement";
        public const String ItemsPanelNotAPanel = "ItemsPanelNotAPanel";
        public const String ItemsPanelNotSingleNode = "ItemsPanelNotSingleNode";
        public const String ItemsSourceInUse = "ItemsSourceInUse";

        public const String ItemTemplateSelectorBreaksDisplayMemberPath =
            "ItemTemplateSelectorBreaksDisplayMemberPath";

        public const String JumpItemsRejectedEventArgs_CountMismatch = "JumpItemsRejectedEventArgs_CountMismatch";
        public const String JumpList_CantApplyUntilEndInit = "JumpList_CantApplyUntilEndInit";
        public const String JumpList_CantCallUnbalancedEndInit = "JumpList_CantCallUnbalancedEndInit";
        public const String JumpList_CantNestBeginInitCalls = "JumpList_CantNestBeginInitCalls";
        public const String KeyAlignCenterDisplayString = "KeyAlignCenterDisplayString";
        public const String KeyAlignJustifyDisplayString = "KeyAlignJustifyDisplayString";
        public const String KeyAlignLeftDisplayString = "KeyAlignLeftDisplayString";
        public const String KeyAlignRightDisplayString = "KeyAlignRightDisplayString";
        public const String KeyAltUndoDisplayString = "KeyAltUndoDisplayString";
        public const String KeyApplyBackground = "KeyApplyBackground";
        public const String KeyApplyBackgroundDisplayString = "KeyApplyBackgroundDisplayString";
        public const String KeyApplyDoubleSpaceDisplayString = "KeyApplyDoubleSpaceDisplayString";
        public const String KeyApplyFontFamily = "KeyApplyFontFamily";
        public const String KeyApplyFontFamilyDisplayString = "KeyApplyFontFamilyDisplayString";
        public const String KeyApplyFontSize = "KeyApplyFontSize";
        public const String KeyApplyFontSizeDisplayString = "KeyApplyFontSizeDisplayString";
        public const String KeyApplyForeground = "KeyApplyForeground";
        public const String KeyApplyForegroundDisplayString = "KeyApplyForegroundDisplayString";
        public const String KeyApplyOneAndAHalfSpaceDisplayString = "KeyApplyOneAndAHalfSpaceDisplayString";
        public const String KeyApplySingleSpaceDisplayString = "KeyApplySingleSpaceDisplayString";
        public const String KeyBackspaceDisplayString = "KeyBackspaceDisplayString";
        public const String KeyCollectionHasInvalidKey = "KeyCollectionHasInvalidKey";
        public const String KeyCopyDisplayString = "KeyCopyDisplayString";
        public const String KeyCopyFormatDisplayString = "KeyCopyFormatDisplayString";
        public const String KeyCorrectionList = "KeyCorrectionList";
        public const String KeyCorrectionListDisplayString = "KeyCorrectionListDisplayString";
        public const String KeyCtrlInsertDisplayString = "KeyCtrlInsertDisplayString";
        public const String KeyCutDisplayString = "KeyCutDisplayString";
        public const String KeyDecreaseFontSizeDisplayString = "KeyDecreaseFontSizeDisplayString";
        public const String KeyDecreaseIndentationDisplayString = "KeyDecreaseIndentationDisplayString";
        public const String KeyDeleteColumnsDisplayString = "KeyDeleteColumnsDisplayString";
        public const String KeyDeleteDisplayString = "KeyDeleteDisplayString";
        public const String KeyDeleteNextWordDisplayString = "KeyDeleteNextWordDisplayString";
        public const String KeyDeletePreviousWordDisplayString = "KeyDeletePreviousWordDisplayString";
        public const String KeyDeleteRows = "KeyDeleteRows";
        public const String KeyDeleteRowsDisplayString = "KeyDeleteRowsDisplayString";
        public const String KeyEnterLineBreakDisplayString = "KeyEnterLineBreakDisplayString";
        public const String KeyEnterParagraphBreakDisplayString = "KeyEnterParagraphBreakDisplayString";
        public const String KeyIncreaseFontSizeDisplayString = "KeyIncreaseFontSizeDisplayString";
        public const String KeyIncreaseIndentationDisplayString = "KeyIncreaseIndentationDisplayString";
        public const String KeyInsertColumnsDisplayString = "KeyInsertColumnsDisplayString";
        public const String KeyInsertRowsDisplayString = "KeyInsertRowsDisplayString";
        public const String KeyInsertTableDisplayString = "KeyInsertTableDisplayString";
        public const String KeyMergeCellsDisplayString = "KeyMergeCellsDisplayString";
        public const String KeyMoveDownByLineDisplayString = "KeyMoveDownByLineDisplayString";
        public const String KeyMoveDownByPageDisplayString = "KeyMoveDownByPageDisplayString";
        public const String KeyMoveDownByParagraphDisplayString = "KeyMoveDownByParagraphDisplayString";
        public const String KeyMoveLeftByCharacterDisplayString = "KeyMoveLeftByCharacterDisplayString";
        public const String KeyMoveLeftByWordDisplayString = "KeyMoveLeftByWordDisplayString";
        public const String KeyMoveRightByCharacterDisplayString = "KeyMoveRightByCharacterDisplayString";
        public const String KeyMoveRightByWordDisplayString = "KeyMoveRightByWordDisplayString";
        public const String KeyMoveToColumnEndDisplayString = "KeyMoveToColumnEndDisplayString";
        public const String KeyMoveToColumnStartDisplayString = "KeyMoveToColumnStartDisplayString";
        public const String KeyMoveToDocumentEndDisplayString = "KeyMoveToDocumentEndDisplayString";
        public const String KeyMoveToDocumentStartDisplayString = "KeyMoveToDocumentStartDisplayString";
        public const String KeyMoveToLineEndDisplayString = "KeyMoveToLineEndDisplayString";
        public const String KeyMoveToLineStartDisplayString = "KeyMoveToLineStartDisplayString";
        public const String KeyMoveToWindowBottomDisplayString = "KeyMoveToWindowBottomDisplayString";
        public const String KeyMoveToWindowTopDisplayString = "KeyMoveToWindowTopDisplayString";
        public const String KeyMoveUpByLineDisplayString = "KeyMoveUpByLineDisplayString";
        public const String KeyMoveUpByPageDisplayString = "KeyMoveUpByPageDisplayString";
        public const String KeyMoveUpByParagraphDisplayString = "KeyMoveUpByParagraphDisplayString";
        public const String KeyPasteFormatDisplayString = "KeyPasteFormatDisplayString";
        public const String KeyRedoDisplayString = "KeyRedoDisplayString";
        public const String KeyRemoveListMarkersDisplayString = "KeyRemoveListMarkersDisplayString";
        public const String KeyResetFormatDisplayString = "KeyResetFormatDisplayString";
        public const String KeySelectAllDisplayString = "KeySelectAllDisplayString";
        public const String KeySelectDownByLineDisplayString = "KeySelectDownByLineDisplayString";
        public const String KeySelectDownByPageDisplayString = "KeySelectDownByPageDisplayString";
        public const String KeySelectDownByParagraphDisplayString = "KeySelectDownByParagraphDisplayString";
        public const String KeySelectLeftByCharacterDisplayString = "KeySelectLeftByCharacterDisplayString";
        public const String KeySelectLeftByWordDisplayString = "KeySelectLeftByWordDisplayString";
        public const String KeySelectRightByCharacterDisplayString = "KeySelectRightByCharacterDisplayString";
        public const String KeySelectRightByWordDisplayString = "KeySelectRightByWordDisplayString";
        public const String KeySelectToColumnEndDisplayString = "KeySelectToColumnEndDisplayString";
        public const String KeySelectToColumnStartDisplayString = "KeySelectToColumnStartDisplayString";
        public const String KeySelectToDocumentEndDisplayString = "KeySelectToDocumentEndDisplayString";
        public const String KeySelectToDocumentStartDisplayString = "KeySelectToDocumentStartDisplayString";
        public const String KeySelectToLineEndDisplayString = "KeySelectToLineEndDisplayString";
        public const String KeySelectToLineStartDisplayString = "KeySelectToLineStartDisplayString";
        public const String KeySelectToWindowBottomDisplayString = "KeySelectToWindowBottomDisplayString";
        public const String KeySelectToWindowTopDisplayString = "KeySelectToWindowTopDisplayString";
        public const String KeySelectUpByLineDisplayString = "KeySelectUpByLineDisplayString";
        public const String KeySelectUpByPageDisplayString = "KeySelectUpByPageDisplayString";
        public const String KeySelectUpByParagraphDisplayString = "KeySelectUpByParagraphDisplayString";
        public const String KeyShiftBackspaceDisplayString = "KeyShiftBackspaceDisplayString";
        public const String KeyShiftDeleteDisplayString = "KeyShiftDeleteDisplayString";
        public const String KeyShiftInsertDisplayString = "KeyShiftInsertDisplayString";
        public const String KeyShiftSpaceDisplayString = "KeyShiftSpaceDisplayString";
        public const String KeySpaceDisplayString = "KeySpaceDisplayString";
        public const String KeySplitCellDisplayString = "KeySplitCellDisplayString";
        public const String KeySwitchViewingModeDisplayString = "KeySwitchViewingModeDisplayString";
        public const String KeyTabBackwardDisplayString = "KeyTabBackwardDisplayString";
        public const String KeyTabForwardDisplayString = "KeyTabForwardDisplayString";
        public const String KeyToggleBoldDisplayString = "KeyToggleBoldDisplayString";
        public const String KeyToggleBulletsDisplayString = "KeyToggleBulletsDisplayString";
        public const String KeyToggleInsertDisplayString = "KeyToggleInsertDisplayString";
        public const String KeyToggleItalicDisplayString = "KeyToggleItalicDisplayString";
        public const String KeyToggleNumberingDisplayString = "KeyToggleNumberingDisplayString";
        public const String KeyToggleSpellCheck = "KeyToggleSpellCheck";
        public const String KeyToggleSpellCheckDisplayString = "KeyToggleSpellCheckDisplayString";
        public const String KeyToggleSubscriptDisplayString = "KeyToggleSubscriptDisplayString";
        public const String KeyToggleSuperscriptDisplayString = "KeyToggleSuperscriptDisplayString";
        public const String KeyToggleUnderlineDisplayString = "KeyToggleUnderlineDisplayString";
        public const String KeyUndoDisplayString = "KeyUndoDisplayString";
        public const String KillBitEnforcedShutdown = "KillBitEnforcedShutdown";
        public const String KnownTypeIdNegative = "KnownTypeIdNegative";
        public const String LengthFormatError = "LengthFormatError";
        public const String ListBoxInvalidAnchorItem = "ListBoxInvalidAnchorItem";
        public const String ListBoxSelectAllKeyDisplayString = "ListBoxSelectAllKeyDisplayString";
        public const String ListBoxSelectAllSelectionMode = "ListBoxSelectAllSelectionMode";
        public const String ListBoxSelectAllText = "ListBoxSelectAllText";
        public const String ListElementItemNotAChildOfList = "ListElementItemNotAChildOfList";

        public const String ListView_GridViewColumnCollectionIsReadOnly =
            "ListView_GridViewColumnCollectionIsReadOnly";

        public const String ListView_IllegalChildrenType = "ListView_IllegalChildrenType";
        public const String ListView_MissingParameterlessConstructor = "ListView_MissingParameterlessConstructor";

        public const String ListView_NotAllowShareColumnToTwoColumnCollection =
            "ListView_NotAllowShareColumnToTwoColumnCollection";

        public const String ListView_ViewCannotBeShared = "ListView_ViewCannotBeShared";
        public const String LogicalTreeLoop = "LogicalTreeLoop";
        public const String LoopDetected = "LoopDetected";
        public const String MarkupExtensionBadStatic = "MarkupExtensionBadStatic";

        public const String MarkupExtensionDynamicOrBindingInCollection =
            "MarkupExtensionDynamicOrBindingInCollection";

        public const String MarkupExtensionDynamicOrBindingOnClrProp = "MarkupExtensionDynamicOrBindingOnClrProp";
        public const String MarkupExtensionNoContext = "MarkupExtensionNoContext";
        public const String MarkupExtensionProperty = "MarkupExtensionProperty";
        public const String MarkupExtensionResourceKey = "MarkupExtensionResourceKey";
        public const String MarkupExtensionResourceNotFound = "MarkupExtensionResourceNotFound";
        public const String MarkupExtensionStaticMember = "MarkupExtensionStaticMember";
        public const String MarkupWriter_CannotSerializeGenerictype = "MarkupWriter_CannotSerializeGenerictype";

        public const String MarkupWriter_CannotSerializeNestedPublictype =
            "MarkupWriter_CannotSerializeNestedPublictype";

        public const String MarkupWriter_CannotSerializeNonPublictype = "MarkupWriter_CannotSerializeNonPublictype";
        public const String MaximumNoteSizeExceeded = "MaximumNoteSizeExceeded";
        public const String MaxLengthExceedsBufferSize = "MaxLengthExceedsBufferSize";
        public const String MeasureReentrancyInvalid = "MeasureReentrancyInvalid";

        public const String MediaElement_CannotSetSourceOnMediaElementDrivenByClock =
            "MediaElement_CannotSetSourceOnMediaElementDrivenByClock";

        public const String MemberNotAllowedDuringAddOrEdit = "MemberNotAllowedDuringAddOrEdit";
        public const String MemberNotAllowedDuringTransaction = "MemberNotAllowedDuringTransaction";
        public const String MemberNotAllowedForView = "MemberNotAllowedForView";
        public const String MissingAnnotationHighlightLayer = "MissingAnnotationHighlightLayer";
        public const String MissingContentSource = "MissingContentSource";
        public const String MissingTagInNamespace = "MissingTagInNamespace";
        public const String MissingTriggerProperty = "MissingTriggerProperty";
        public const String MissingValueConverter = "MissingValueConverter";
        public const String ModificationEarlierThanCreation = "ModificationEarlierThanCreation";

        public const String ModifyingLogicalTreeViaStylesNotImplemented =
            "ModifyingLogicalTreeViaStylesNotImplemented";

        public const String MoreThanOneAttachedAnnotation = "MoreThanOneAttachedAnnotation";
        public const String MoreThanOneStartingParts = "MoreThanOneStartingParts";
        public const String MoveInDeferSelectionActive = "MoveInDeferSelectionActive";
        public const String MultiBindingHasNoConverter = "MultiBindingHasNoConverter";
        public const String MultipleAssemblyMatches = "MultipleAssemblyMatches";
        public const String MultiSelectorSelectAll = "MultiSelectorSelectAll";
        public const String MultiSingleton = "MultiSingleton";
        public const String MultiThreadedCollectionChangeNotSupported = "MultiThreadedCollectionChangeNotSupported";
        public const String MustBaseOnStyleOfABaseType = "MustBaseOnStyleOfABaseType";
        public const String MustBeCondition = "MustBeCondition";
        public const String MustBeFrameworkDerived = "MustBeFrameworkDerived";
        public const String MustBeFrameworkOr3DDerived = "MustBeFrameworkOr3DDerived";
        public const String MustBeOfType = "MustBeOfType";
        public const String MustBeTriggerAction = "MustBeTriggerAction";
        public const String MustBeTypeOrString = "MustBeTypeOrString";
        public const String MustImplementIUriContext = "MustImplementIUriContext";
        public const String MustNotTemplateUnassociatedControl = "MustNotTemplateUnassociatedControl";
        public const String MustUseWindowStyleNone = "MustUseWindowStyleNone";
        public const String NamedObjectMustBeFrameworkElement = "NamedObjectMustBeFrameworkElement";
        public const String NameNotEmptyString = "NameNotEmptyString";
        public const String NameNotFound = "NameNotFound";
        public const String NameScopeDuplicateNamesNotAllowed = "NameScopeDuplicateNamesNotAllowed";
        public const String NameScopeInvalidIdentifierName = "NameScopeInvalidIdentifierName";
        public const String NameScopeNameNotEmptyString = "NameScopeNameNotEmptyString";
        public const String NameScopeNameNotFound = "NameScopeNameNotFound";
        public const String NameScopeNotFound = "NameScopeNotFound";
        public const String NamesNotSupportedInsideResourceDictionary = "NamesNotSupportedInsideResourceDictionary";
        public const String NavWindowMenuCurrentPage = "NavWindowMenuCurrentPage";
        public const String NeedToBeComVisible = "NeedToBeComVisible";
        public const String NegativeValue = "NegativeValue";
        public const String NestedBeginInitNotSupported = "NestedBeginInitNotSupported";
        public const String NoAddChild = "NoAddChild";
        public const String NoAttachedAnnotationToModify = "NoAttachedAnnotationToModify";
        public const String NoBackEntry = "NoBackEntry";
        public const String NoCheckOrChangeWhenDeferred = "NoCheckOrChangeWhenDeferred";
        public const String NoDefaultUpdateSourceTrigger = "NoDefaultUpdateSourceTrigger";
        public const String NoElement = "NoElement";
        public const String NoElementObject = "NoElementObject";
        public const String NoForwardEntry = "NoForwardEntry";
        public const String NoMulticastHandlers = "NoMulticastHandlers";
        public const String NonClsActivationException = "NonClsActivationException";
        public const String NonCLSException = "NonCLSException";
        public const String NonPackAppAbsoluteUriNotAllowed = "NonPackAppAbsoluteUriNotAllowed";
        public const String NonPackSooAbsoluteUriNotAllowed = "NonPackSooAbsoluteUriNotAllowed";
        public const String NonWhiteSpaceInAddText = "NonWhiteSpaceInAddText";
        public const String NoPresentationContextForGivenElement = "NoPresentationContextForGivenElement";
        public const String NoProcessorForSelectionType = "NoProcessorForSelectionType";
        public const String NoScopingElement = "NoScopingElement";
        public const String NotAllowedBeforeShow = "NotAllowedBeforeShow";
        public const String NotHighlightAnnotationType = "NotHighlightAnnotationType";
        public const String NotInAssociatedContainer = "NotInAssociatedContainer";
        public const String NotInAssociatedTree = "NotInAssociatedTree";
        public const String NotInThisTree = "NotInThisTree";
        public const String NotSupported = "NotSupported";
        public const String NotSupportedInBrowser = "NotSupportedInBrowser";

        public const String NoUpdateSourceTriggerForInnerBindingOfMultiBinding =
            "NoUpdateSourceTriggerForInnerBindingOfMultiBinding";

        public const String NullParentNode = "NullParentNode";
        public const String NullPropertyIllegal = "NullPropertyIllegal";
        public const String NullTypeIllegal = "NullTypeIllegal";
        public const String NullUri = "NullUri";
        public const String ObjectDataProviderCanHaveOnlyOneSource = "ObjectDataProviderCanHaveOnlyOneSource";
        public const String ObjectDataProviderHasNoSource = "ObjectDataProviderHasNoSource";
        public const String ObjectDataProviderNonCLSException = "ObjectDataProviderNonCLSException";
        public const String ObjectDataProviderNonCLSExceptionInvoke = "ObjectDataProviderNonCLSExceptionInvoke";

        public const String ObjectDataProviderParameterCollectionIsNotInUse =
            "ObjectDataProviderParameterCollectionIsNotInUse";

        public const String ObjectDisposed_StoreClosed = "ObjectDisposed_StoreClosed";
        public const String OnlyFlowAndFixedSupported = "OnlyFlowAndFixedSupported";
        public const String OnlyFlowFixedSupported = "OnlyFlowFixedSupported";
        public const String PageCacheSizeNotAllowed = "PageCacheSizeNotAllowed";
        public const String PageCannotHaveMultipleContent = "PageCannotHaveMultipleContent";
        public const String PageContentNotFound = "PageContentNotFound";
        public const String PageContentUnsupportedMimeType = "PageContentUnsupportedMimeType";
        public const String PageContentUnsupportedPageType = "PageContentUnsupportedPageType";
        public const String PanelIsNotItemsHost = "PanelIsNotItemsHost";
        public const String Panel_BoundPanel_NoChildren = "Panel_BoundPanel_NoChildren";
        public const String Panel_ItemsControlNotFound = "Panel_ItemsControlNotFound";
        public const String Panel_NoNullChildren = "Panel_NoNullChildren";
        public const String Panel_NoNullVisualParent = "Panel_NoNullVisualParent";
        public const String ParameterMustBeLogicalNode = "ParameterMustBeLogicalNode";
        public const String ParentOfPageMustBeWindowOrFrame = "ParentOfPageMustBeWindowOrFrame";
        public const String ParserAbandonedTypeConverterText = "ParserAbandonedTypeConverterText";
        public const String ParserAsyncOnRoot = "ParserAsyncOnRoot";
        public const String ParserAttachedPropInheritError = "ParserAttachedPropInheritError";
        public const String ParserAttributeArgsLow = "ParserAttributeArgsLow";
        public const String ParserAttributeNamespaceMisMatch = "ParserAttributeNamespaceMisMatch";
        public const String ParserBadAssemblyName = "ParserBadAssemblyName";
        public const String ParserBadAssemblyPath = "ParserBadAssemblyPath";
        public const String ParserBadChild = "ParserBadChild";
        public const String ParserBadConstructorParams = "ParserBadConstructorParams";
        public const String ParserBadEncoding = "ParserBadEncoding";
        public const String ParserBadKey = "ParserBadKey";
        public const String ParserBadMemberReference = "ParserBadMemberReference";
        public const String ParserBadName = "ParserBadName";
        public const String ParserBadNullableType = "ParserBadNullableType";
        public const String ParserBadString = "ParserBadString";
        public const String ParserBadSyncMode = "ParserBadSyncMode";
        public const String ParserBadTypeInArrayProperty = "ParserBadTypeInArrayProperty";
        public const String ParserBadUidOrNameME = "ParserBadUidOrNameME";
        public const String ParserBamlEvent = "ParserBamlEvent";
        public const String ParserBamlVersion = "ParserBamlVersion";
        public const String ParserCannotAddAnyChildren = "ParserCannotAddAnyChildren";
        public const String ParserCannotAddAnyChildren2 = "ParserCannotAddAnyChildren2";
        public const String ParserCannotAddChild = "ParserCannotAddChild";
        public const String ParserCannotConvertInitializationText = "ParserCannotConvertInitializationText";
        public const String ParserCannotConvertPropertyValue = "ParserCannotConvertPropertyValue";
        public const String ParserCannotConvertPropertyValueString = "ParserCannotConvertPropertyValueString";
        public const String ParserCannotConvertString = "ParserCannotConvertString";
        public const String ParserCannotReuseXamlReader = "ParserCannotReuseXamlReader";
        public const String ParserCannotSetValue = "ParserCannotSetValue";
        public const String ParserCanOnlyHaveOneChild = "ParserCanOnlyHaveOneChild";
        public const String ParserCantCreateDelegate = "ParserCantCreateDelegate";
        public const String ParserCantCreateInstanceType = "ParserCantCreateInstanceType";
        public const String ParserCantCreateTextComplexProp = "ParserCantCreateTextComplexProp";
        public const String ParserCantGetDPOrPi = "ParserCantGetDPOrPi";
        public const String ParserCantGetProperty = "ParserCantGetProperty";
        public const String ParserCantSetAttribute = "ParserCantSetAttribute";
        public const String ParserCantSetContentProperty = "ParserCantSetContentProperty";
        public const String ParserCantSetTriggerCondition = "ParserCantSetTriggerCondition";
        public const String ParserCompatDuplicate = "ParserCompatDuplicate";
        public const String ParserContentMustBeContiguous = "ParserContentMustBeContiguous";
        public const String ParserDefaultConverterElement = "ParserDefaultConverterElement";
        public const String ParserDefaultConverterProperty = "ParserDefaultConverterProperty";
        public const String ParserDeferContentAsync = "ParserDeferContentAsync";
        public const String ParserDefSharedOnlyInCompiled = "ParserDefSharedOnlyInCompiled";
        public const String ParserDefTag = "ParserDefTag";
        public const String ParserDictionarySealed = "ParserDictionarySealed";
        public const String ParserDupDictionaryKey = "ParserDupDictionaryKey";
        public const String ParserDuplicateMarkupExtensionProperty = "ParserDuplicateMarkupExtensionProperty";
        public const String ParserDuplicateProperty1 = "ParserDuplicateProperty1";
        public const String ParserDuplicateProperty2 = "ParserDuplicateProperty2";
        public const String ParserEmptyComplexProp = "ParserEmptyComplexProp";
        public const String ParserEntityReference = "ParserEntityReference";
        public const String ParserErrorContext_File = "ParserErrorContext_File";
        public const String ParserErrorContext_File_Line = "ParserErrorContext_File_Line";
        public const String ParserErrorContext_Line = "ParserErrorContext_Line";
        public const String ParserErrorContext_Type = "ParserErrorContext_Type";
        public const String ParserErrorContext_Type_File = "ParserErrorContext_Type_File";
        public const String ParserErrorContext_Type_File_Line = "ParserErrorContext_Type_File_Line";
        public const String ParserErrorContext_Type_Line = "ParserErrorContext_Type_Line";
        public const String ParserErrorCreatingInstance = "ParserErrorCreatingInstance";
        public const String ParserErrorParsingAttrib = "ParserErrorParsingAttrib";
        public const String ParserErrorParsingAttribType = "ParserErrorParsingAttribType";
        public const String ParserEventDelegateTypeNotAccessible = "ParserEventDelegateTypeNotAccessible";
        public const String ParserFailedEndInit = "ParserFailedEndInit";
        public const String ParserFailedToCreateFromConstructor = "ParserFailedToCreateFromConstructor";
        public const String ParserFailFindType = "ParserFailFindType";

        public const String ParserFilterXmlReaderNoDefinitionPrefixChangeAllowed =
            "ParserFilterXmlReaderNoDefinitionPrefixChangeAllowed";

        public const String ParserFilterXmlReaderNoIndexAttributeAccess =
            "ParserFilterXmlReaderNoIndexAttributeAccess";

        public const String ParserIAddChildText = "ParserIAddChildText";
        public const String ParserIEnumerableIAddChild = "ParserIEnumerableIAddChild";
        public const String ParserInvalidContentPropertyAttribute = "ParserInvalidContentPropertyAttribute";
        public const String ParserInvalidIdentifierName = "ParserInvalidIdentifierName";
        public const String ParserInvalidStaticMember = "ParserInvalidStaticMember";
        public const String ParserKeyOnExplicitDictionary = "ParserKeyOnExplicitDictionary";
        public const String ParserKeysAreStrings = "ParserKeysAreStrings";
        public const String ParserLineAndOffset = "ParserLineAndOffset";
        public const String ParserMapPIMissingAssembly = "ParserMapPIMissingAssembly";
        public const String ParserMapPIMissingKey = "ParserMapPIMissingKey";
        public const String ParserMappingUriInvalid = "ParserMappingUriInvalid";
        public const String ParserMarkupExtensionBadConstructorParam = "ParserMarkupExtensionBadConstructorParam";
        public const String ParserMarkupExtensionBadDelimiter = "ParserMarkupExtensionBadDelimiter";

        public const String ParserMarkupExtensionDelimiterBeforeFirstAttribute =
            "ParserMarkupExtensionDelimiterBeforeFirstAttribute";

        public const String ParserMarkupExtensionInvalidClosingBracketCharacers =
            "ParserMarkupExtensionInvalidClosingBracketCharacers";

        public const String ParserMarkupExtensionMalformedBracketCharacers =
            "ParserMarkupExtensionMalformedBracketCharacers";

        public const String ParserMarkupExtensionNoEndCurlie = "ParserMarkupExtensionNoEndCurlie";
        public const String ParserMarkupExtensionNoNameValue = "ParserMarkupExtensionNoNameValue";
        public const String ParserMarkupExtensionNoQuotesInName = "ParserMarkupExtensionNoQuotesInName";
        public const String ParserMarkupExtensionTrailingGarbage = "ParserMarkupExtensionTrailingGarbage";
        public const String ParserMarkupExtensionUnknownAttr = "ParserMarkupExtensionUnknownAttr";
        public const String ParserMetroUnknownAttribute = "ParserMetroUnknownAttribute";
        public const String ParserMultiBamls = "ParserMultiBamls";
        public const String ParserMultiRoot = "ParserMultiRoot";
        public const String ParserNestedComplexProp = "ParserNestedComplexProp";
        public const String ParserNoAttrArray = "ParserNoAttrArray";
        public const String ParserNoChildrenTag = "ParserNoChildrenTag";
        public const String ParserNoComplexMulti = "ParserNoComplexMulti";
        public const String ParserNoDefaultConstructor = "ParserNoDefaultConstructor";
        public const String ParserNoDefaultPropConstructor = "ParserNoDefaultPropConstructor";
        public const String ParserNoDictionaryKey = "ParserNoDictionaryKey";
        public const String ParserNoDictionaryName = "ParserNoDictionaryName";
        public const String ParserNoDigitEnums = "ParserNoDigitEnums";
        public const String ParserNoDPOnOwner = "ParserNoDPOnOwner";
        public const String ParserNoElementCreate2 = "ParserNoElementCreate2";
        public const String ParserNoEvents = "ParserNoEvents";
        public const String ParserNoEventTag = "ParserNoEventTag";
        public const String ParserNoMatchingArray = "ParserNoMatchingArray";
        public const String ParserNoMatchingIDictionary = "ParserNoMatchingIDictionary";
        public const String ParserNoMatchingIList = "ParserNoMatchingIList";
        public const String ParserNoNameOnType = "ParserNoNameOnType";
        public const String ParserNoNamespace = "ParserNoNamespace";
        public const String ParserNoNameUnderDefinitionScopeType = "ParserNoNameUnderDefinitionScopeType";
        public const String ParserNoNestedXmlDataIslands = "ParserNoNestedXmlDataIslands";
        public const String ParserNoPropOnComplexProp = "ParserNoPropOnComplexProp";
        public const String ParserNoPropType = "ParserNoPropType";
        public const String ParserNoResource = "ParserNoResource";
        public const String ParserNoSerializer = "ParserNoSerializer";
        public const String ParserNoSetterChild = "ParserNoSetterChild";
        public const String ParserNotAllowedpublicType = "ParserNotAllowedpublicType";
        public const String ParserNotMarkedPublic = "ParserNotMarkedPublic";
        public const String ParserNotMarkupExtension = "ParserNotMarkupExtension";
        public const String ParserNoType = "ParserNoType";
        public const String ParserNoTypeConv = "ParserNoTypeConv";
        public const String ParserNullPropertyCollection = "ParserNullPropertyCollection";
        public const String ParserNullReturned = "ParserNullReturned";
        public const String ParserOwnerEventMustBePublic = "ParserOwnerEventMustBePublic";
        public const String ParserParentDO = "ParserParentDO";
        public const String ParserPrefixNSElement = "ParserPrefixNSElement";
        public const String ParserPrefixNSProperty = "ParserPrefixNSProperty";
        public const String ParserPropertyCollectionClosed = "ParserPropertyCollectionClosed";
        public const String ParserPropNoValue = "ParserPropNoValue";
        public const String ParserProvideValueCantSetUri = "ParserProvideValueCantSetUri";
        public const String ParserPublicType = "ParserPublicType";
        public const String ParserReadOnlyNullProperty = "ParserReadOnlyNullProperty";
        public const String ParserReadOnlyProp = "ParserReadOnlyProp";
        public const String ParserResourceKeyType = "ParserResourceKeyType";
        public const String ParserSyncOnRoot = "ParserSyncOnRoot";
        public const String ParserTextInComplexProp = "ParserTextInComplexProp";
        public const String ParserTextInvalidInArrayOrDictionary = "ParserTextInvalidInArrayOrDictionary";
        public const String ParserTooManyAssemblies = "ParserTooManyAssemblies";
        public const String ParserTypeConverterTextNeedsEndElement = "ParserTypeConverterTextNeedsEndElement";
        public const String ParserTypeConverterTextUnusable = "ParserTypeConverterTextUnusable";
        public const String ParserUndeclaredNS = "ParserUndeclaredNS";
        public const String ParserUnexpectedEndEle = "ParserUnexpectedEndEle";
        public const String ParserUnexpInBAML = "ParserUnexpInBAML";
        public const String ParserUnknownAttribute = "ParserUnknownAttribute";
        public const String ParserUnknownBaml = "ParserUnknownBaml";
        public const String ParserUnknownDefAttribute = "ParserUnknownDefAttribute";
        public const String ParserUnknownDefAttributeCompiler = "ParserUnknownDefAttributeCompiler";
        public const String ParserUnknownPresentationOptionsAttribute = "ParserUnknownPresentationOptionsAttribute";
        public const String ParserUnknownTag = "ParserUnknownTag";
        public const String ParserUnknownXmlType = "ParserUnknownXmlType";
        public const String ParserWriterNoSeekEnd = "ParserWriterNoSeekEnd";
        public const String ParserWriterUnknownOrigin = "ParserWriterUnknownOrigin";
        public const String ParserXmlIslandMissing = "ParserXmlIslandMissing";
        public const String ParserXmlIslandUnexpected = "ParserXmlIslandUnexpected";
        public const String ParserXmlLangPropertyValueInvalid = "ParserXmlLangPropertyValueInvalid";
        public const String ParserXmlReaderNoLineInfo = "ParserXmlReaderNoLineInfo";
        public const String PartialTrustPrintDialogMustBeInvoked = "PartialTrustPrintDialogMustBeInvoked";
        public const String PasswordBoxInvalidTextContainer = "PasswordBoxInvalidTextContainer";
        public const String PathParameterIsNull = "PathParameterIsNull";
        public const String PathParametersIndexOutOfRange = "PathParametersIndexOutOfRange";
        public const String PathSyntax = "PathSyntax";
        public const String PlatformRequirementTitle = "PlatformRequirementTitle";
        public const String PopupReopeningNotAllowed = "PopupReopeningNotAllowed";
        public const String PositionalArgumentsWrongLength = "PositionalArgumentsWrongLength";
        public const String PrevoiusPartialPageContentOutstanding = "PrevoiusPartialPageContentOutstanding";

        public const String PrevoiusUninitializedDocumentReferenceOutstanding =
            "PrevoiusUninitializedDocumentReferenceOutstanding";

        public const String PrintDialogInstallPrintSupportCaption = "PrintDialogInstallPrintSupportCaption";
        public const String PrintDialogInstallPrintSupportMessageBox = "PrintDialogInstallPrintSupportMessageBox";
        public const String PrintDialogInvalidPageRange = "PrintDialogInvalidPageRange";
        public const String PrintDialogPageRange = "PrintDialogPageRange";
        public const String PrintDialogZeroNotAllowed = "PrintDialogZeroNotAllowed";
        public const String PrintJobDescription = "PrintJobDescription";
        public const String ProgressBarReadOnly = "ProgressBarReadOnly";
        public const String PropertyFoundOutsideStartElement = "PropertyFoundOutsideStartElement";
        public const String PropertyIdOutOfSequence = "PropertyIdOutOfSequence";
        public const String PropertyIsImmutable = "PropertyIsImmutable";
        public const String PropertyIsInitializeOnly = "PropertyIsInitializeOnly";
        public const String PropertyMustHaveValue = "PropertyMustHaveValue";
        public const String PropertyNotBindable = "PropertyNotBindable";
        public const String PropertyNotFound = "PropertyNotFound";
        public const String PropertyNotSupported = "PropertyNotSupported";
        public const String PropertyOutOfOrder = "PropertyOutOfOrder";
        public const String PropertyPathIndexWrongType = "PropertyPathIndexWrongType";
        public const String PropertyPathInvalidAccessor = "PropertyPathInvalidAccessor";
        public const String PropertyPathNoOwnerType = "PropertyPathNoOwnerType";
        public const String PropertyPathNoProperty = "PropertyPathNoProperty";
        public const String PropertyPathSyntaxError = "PropertyPathSyntaxError";
        public const String PropertyToSortByNotFoundOnType = "PropertyToSortByNotFoundOnType";
        public const String PropertyTriggerCycleDetected = "PropertyTriggerCycleDetected";
        public const String PropertyTriggerLayerLimitExceeded = "PropertyTriggerLayerLimitExceeded";
        public const String PTSError = "PTSError";
        public const String PTSInvalidHandle = "PTSInvalidHandle";
        public const String RangeActionsNotSupported = "RangeActionsNotSupported";
        public const String ReadCountNegative = "ReadCountNegative";
        public const String ReadNotSupported = "ReadNotSupported";
        public const String ReadOnlyPropertyNotAllowed = "ReadOnlyPropertyNotAllowed";
        public const String RecordOutOfOrder = "RecordOutOfOrder";
        public const String Rect_WidthAndHeightCannotBeNegative = "Rect_WidthAndHeightCannotBeNegative";
        public const String RelativeSourceInvalidAncestorLevel = "RelativeSourceInvalidAncestorLevel";
        public const String RelativeSourceModeInvalid = "RelativeSourceModeInvalid";
        public const String RelativeSourceModeIsImmutable = "RelativeSourceModeIsImmutable";
        public const String RelativeSourceNeedsAncestorType = "RelativeSourceNeedsAncestorType";
        public const String RelativeSourceNeedsMode = "RelativeSourceNeedsMode";
        public const String RelativeSourceNotInFindAncestorMode = "RelativeSourceNotInFindAncestorMode";
        public const String RemovedItemNotFound = "RemovedItemNotFound";
        public const String RemoveRequiresOffsetZero = "RemoveRequiresOffsetZero";
        public const String RemoveRequiresPositiveCount = "RemoveRequiresPositiveCount";
        public const String RemovingPlaceholder = "RemovingPlaceholder";
        public const String ReparentModelChildIllegal = "ReparentModelChildIllegal";
        public const String RequestNavigateEventMustHaveRoutedEvent = "RequestNavigateEventMustHaveRoutedEvent";
        public const String RequiredAttributeMissing = "RequiredAttributeMissing";
        public const String RequiresExplicitCulture = "RequiresExplicitCulture";
        public const String RequiresXmlNamespaceMapping = "RequiresXmlNamespaceMapping";
        public const String RequiresXmlNamespaceMappingUri = "RequiresXmlNamespaceMappingUri";
        public const String ReshowNotAllowed = "ReshowNotAllowed";
        public const String ResourceDictionaryDeferredContentFailure = "ResourceDictionaryDeferredContentFailure";
        public const String ResourceDictionaryDuplicateDeferredContent = "ResourceDictionaryDuplicateDeferredContent";
        public const String ResourceDictionaryInvalidMergedDictionary = "ResourceDictionaryInvalidMergedDictionary";
        public const String ResourceDictionaryIsReadOnly = "ResourceDictionaryIsReadOnly";
        public const String ResourceDictionaryLoadFromFailure = "ResourceDictionaryLoadFromFailure";
        public const String ReturnEventHandlerMustBeOnParentPage = "ReturnEventHandlerMustBeOnParentPage";

        public const String RichTextBox_CantSetDocumentInsideChangeBlock =
            "RichTextBox_CantSetDocumentInsideChangeBlock";

        public const String RichTextBox_DocumentBelongsToAnotherRichTextBoxAlready =
            "RichTextBox_DocumentBelongsToAnotherRichTextBoxAlready";

        public const String RichTextBox_PointerNotInSameDocument = "RichTextBox_PointerNotInSameDocument";
        public const String RowCacheCannotModifyNonExistentLayout = "RowCacheCannotModifyNonExistentLayout";
        public const String RowCachePageNotFound = "RowCachePageNotFound";
        public const String RowCacheRecalcWithNoPageCache = "RowCacheRecalcWithNoPageCache";
        public const String RuntimeTypeRequired = "RuntimeTypeRequired";
        public const String ScrollBar_ContextMenu_Bottom = "ScrollBar_ContextMenu_Bottom";
        public const String ScrollBar_ContextMenu_LeftEdge = "ScrollBar_ContextMenu_LeftEdge";
        public const String ScrollBar_ContextMenu_PageDown = "ScrollBar_ContextMenu_PageDown";
        public const String ScrollBar_ContextMenu_PageLeft = "ScrollBar_ContextMenu_PageLeft";
        public const String ScrollBar_ContextMenu_PageRight = "ScrollBar_ContextMenu_PageRight";
        public const String ScrollBar_ContextMenu_PageUp = "ScrollBar_ContextMenu_PageUp";
        public const String ScrollBar_ContextMenu_RightEdge = "ScrollBar_ContextMenu_RightEdge";
        public const String ScrollBar_ContextMenu_ScrollDown = "ScrollBar_ContextMenu_ScrollDown";
        public const String ScrollBar_ContextMenu_ScrollHere = "ScrollBar_ContextMenu_ScrollHere";
        public const String ScrollBar_ContextMenu_ScrollLeft = "ScrollBar_ContextMenu_ScrollLeft";
        public const String ScrollBar_ContextMenu_ScrollRight = "ScrollBar_ContextMenu_ScrollRight";
        public const String ScrollBar_ContextMenu_ScrollUp = "ScrollBar_ContextMenu_ScrollUp";
        public const String ScrollBar_ContextMenu_Top = "ScrollBar_ContextMenu_Top";
        public const String ScrollViewer_CannotBeNaN = "ScrollViewer_CannotBeNaN";
        public const String ScrollViewer_OutOfRange = "ScrollViewer_OutOfRange";
        public const String SeekFailed = "SeekFailed";
        public const String SeekNegative = "SeekNegative";
        public const String SeekNotSupported = "SeekNotSupported";
        public const String SelectedCellsCollection_DuplicateItem = "SelectedCellsCollection_DuplicateItem";
        public const String SelectedCellsCollection_InvalidItem = "SelectedCellsCollection_InvalidItem";
        public const String SelectionChangeActive = "SelectionChangeActive";
        public const String SelectionChangeNotActive = "SelectionChangeNotActive";
        public const String SelectionDoesNotResolveToAPage = "SelectionDoesNotResolveToAPage";
        public const String SelectionMustBeServiceProvider = "SelectionMustBeServiceProvider";
        public const String SerializerProviderAlreadyRegistered = "SerializerProviderAlreadyRegistered";
        public const String SerializerProviderCannotLoad = "SerializerProviderCannotLoad";
        public const String SerializerProviderDefaultFileExtensionNull = "SerializerProviderDefaultFileExtensionNull";
        public const String SerializerProviderDisplayNameNull = "SerializerProviderDisplayNameNull";
        public const String SerializerProviderManufacturerNameNull = "SerializerProviderManufacturerNameNull";
        public const String SerializerProviderManufacturerWebsiteNull = "SerializerProviderManufacturerWebsiteNull";
        public const String SerializerProviderNotRegistered = "SerializerProviderNotRegistered";
        public const String SerializerProviderUnknownSerializer = "SerializerProviderUnknownSerializer";
        public const String SerializerProviderWrongVersion = "SerializerProviderWrongVersion";
        public const String SetFocusFailed = "SetFocusFailed";
        public const String SetInDeferSelectionActive = "SetInDeferSelectionActive";
        public const String SetLengthNotSupported = "SetLengthNotSupported";
        public const String SetPositionNotSupported = "SetPositionNotSupported";
        public const String SetterOnStyleNotAllowedToHaveTarget = "SetterOnStyleNotAllowedToHaveTarget";
        public const String SetterValueCannotBeUnset = "SetterValueCannotBeUnset";
        public const String SetterValueOfMarkupExtensionNotSupported = "SetterValueOfMarkupExtensionNotSupported";
        public const String SharedAttributeInLooseXaml = "SharedAttributeInLooseXaml";
        public const String ShowDialogOnModal = "ShowDialogOnModal";
        public const String ShowDialogOnVisible = "ShowDialogOnVisible";
        public const String ShowNonActivatedAndMaximized = "ShowNonActivatedAndMaximized";
        public const String ShutdownModeWhenAppShutdown = "ShutdownModeWhenAppShutdown";
        public const String SourceNameNotSupportedForDataTriggers = "SourceNameNotSupportedForDataTriggers";
        public const String SourceNameNotSupportedForStyleTriggers = "SourceNameNotSupportedForStyleTriggers";
        public const String Stack_VisualInDifferentSubTree = "Stack_VisualInDifferentSubTree";
        public const String StartIndexExceedsBufferSize = "StartIndexExceedsBufferSize";
        public const String StartNodeMustBeDocumentPageViewOrFixedPage = "StartNodeMustBeDocumentPageViewOrFixedPage";
        public const String StartNodeMustBeFixedPageProxy = "StartNodeMustBeFixedPageProxy";
        public const String StaticResourceInXamlOnly = "StaticResourceInXamlOnly";
        public const String Storyboard_AnimationMismatch = "Storyboard_AnimationMismatch";
        public const String Storyboard_BeginStoryboardNameNotFound = "Storyboard_BeginStoryboardNameNotFound";
        public const String Storyboard_BeginStoryboardNameRequired = "Storyboard_BeginStoryboardNameRequired";
        public const String Storyboard_BeginStoryboardNoStoryboard = "Storyboard_BeginStoryboardNoStoryboard";
        public const String Storyboard_ComplexPathNotSupported = "Storyboard_ComplexPathNotSupported";
        public const String Storyboard_ImmutableTargetNotSupported = "Storyboard_ImmutableTargetNotSupported";
        public const String Storyboard_MediaElementNotFound = "Storyboard_MediaElementNotFound";
        public const String Storyboard_MediaElementRequired = "Storyboard_MediaElementRequired";
        public const String Storyboard_NameNotFound = "Storyboard_NameNotFound";
        public const String Storyboard_NeverApplied = "Storyboard_NeverApplied";
        public const String Storyboard_NoNameScope = "Storyboard_NoNameScope";
        public const String Storyboard_NoTarget = "Storyboard_NoTarget";
        public const String Storyboard_PropertyPathEmpty = "Storyboard_PropertyPathEmpty";
        public const String Storyboard_PropertyPathFrozenCheckFailed = "Storyboard_PropertyPathFrozenCheckFailed";

        public const String Storyboard_PropertyPathIncludesNonAnimatableProperty =
            "Storyboard_PropertyPathIncludesNonAnimatableProperty";

        public const String Storyboard_PropertyPathMustPointToDependencyObject =
            "Storyboard_PropertyPathMustPointToDependencyObject";

        public const String Storyboard_PropertyPathMustPointToDependencyProperty =
            "Storyboard_PropertyPathMustPointToDependencyProperty";

        public const String Storyboard_PropertyPathObjectNotFound = "Storyboard_PropertyPathObjectNotFound";
        public const String Storyboard_PropertyPathPropertyNotFound = "Storyboard_PropertyPathPropertyNotFound";
        public const String Storyboard_PropertyPathSealedCheckFailed = "Storyboard_PropertyPathSealedCheckFailed";
        public const String Storyboard_PropertyPathUnresolved = "Storyboard_PropertyPathUnresolved";
        public const String Storyboard_StoryboardReferenceRequired = "Storyboard_StoryboardReferenceRequired";
        public const String Storyboard_TargetNameNotAllowedInStyle = "Storyboard_TargetNameNotAllowedInStyle";
        public const String Storyboard_TargetNameNotDependencyObject = "Storyboard_TargetNameNotDependencyObject";
        public const String Storyboard_TargetPropertyRequired = "Storyboard_TargetPropertyRequired";
        public const String Storyboard_UnableToFreeze = "Storyboard_UnableToFreeze";
        public const String Storyboard_UnrecognizedHandoffBehavior = "Storyboard_UnrecognizedHandoffBehavior";
        public const String Storyboard_UnrecognizedTimeSeekOrigin = "Storyboard_UnrecognizedTimeSeekOrigin";
        public const String StreamCannotBeWritten = "StreamCannotBeWritten";
        public const String StreamDoesNotSupportSeek = "StreamDoesNotSupportSeek";
        public const String StreamDoesNotSupportWrite = "StreamDoesNotSupportWrite";
        public const String StreamNotSet = "StreamNotSet";
        public const String StreamObjectDisposed = "StreamObjectDisposed";
        public const String StringIdOutOfSequence = "StringIdOutOfSequence";
        public const String StyleBasedOnHasLoop = "StyleBasedOnHasLoop";
        public const String StyleCannotBeBasedOnSelf = "StyleCannotBeBasedOnSelf";
        public const String StyleDataTriggerBindingHasBadValue = "StyleDataTriggerBindingHasBadValue";
        public const String StyleDataTriggerBindingMissing = "StyleDataTriggerBindingMissing";
        public const String StyleForWrongType = "StyleForWrongType";
        public const String StyleHasTooManyElements = "StyleHasTooManyElements";
        public const String StyleImpliedAndComplexChildren = "StyleImpliedAndComplexChildren";
        public const String StyleInvalidElementTag = "StyleInvalidElementTag";
        public const String StyleKnownTagWrongLocation = "StyleKnownTagWrongLocation";
        public const String StyleNoClrEvent = "StyleNoClrEvent";
        public const String StyleNoDef = "StyleNoDef";
        public const String StyleNoDictionaryKey = "StyleNoDictionaryKey";
        public const String StyleNoEventSetters = "StyleNoEventSetters";
        public const String StyleNoPropOrEvent = "StyleNoPropOrEvent";
        public const String StyleNoSetterResource = "StyleNoSetterResource";
        public const String StyleNoTarget = "StyleNoTarget";
        public const String StyleNoTemplateBindInSetters = "StyleNoTemplateBindInSetters";
        public const String StyleNoTemplateBindInVisualTrigger = "StyleNoTemplateBindInVisualTrigger";
        public const String StyleNoTopLevelElement = "StyleNoTopLevelElement";
        public const String StylePropertyCustom = "StylePropertyCustom";
        public const String StylePropertyInStyleNotAllowed = "StylePropertyInStyleNotAllowed";
        public const String StylePropertySetterMinAttrs = "StylePropertySetterMinAttrs";
        public const String StylePropTriggerPropMissing = "StylePropTriggerPropMissing";
        public const String StyleSetterUnknownProp = "StyleSetterUnknownProp";
        public const String StyleTagNotSupported = "StyleTagNotSupported";
        public const String StyleTargetTypeMismatchWithElement = "StyleTargetTypeMismatchWithElement";
        public const String StyleTextNotSupported = "StyleTextNotSupported";
        public const String StyleTriggersCannotTargetTheTemplate = "StyleTriggersCannotTargetTheTemplate";
        public const String StyleUnknownProp = "StyleUnknownProp";
        public const String StyleUnknownTrigger = "StyleUnknownTrigger";
        public const String StyleValueOfExpressionNotSupported = "StyleValueOfExpressionNotSupported";
        public const String SystemResourceForTypeIsNotStyle = "SystemResourceForTypeIsNotStyle";
        public const String TableCollectionCountNeedNonNegNum = "TableCollectionCountNeedNonNegNum";
        public const String TableCollectionElementTypeExpected = "TableCollectionElementTypeExpected";
        public const String TableCollectionInOtherCollection = "TableCollectionInOtherCollection";
        public const String TableCollectionInvalidOffLen = "TableCollectionInvalidOffLen";
        public const String TableCollectionNotEnoughCapacity = "TableCollectionNotEnoughCapacity";
        public const String TableCollectionOutOfRange = "TableCollectionOutOfRange";
        public const String TableCollectionOutOfRangeNeedNonNegNum = "TableCollectionOutOfRangeNeedNonNegNum";
        public const String TableCollectionRangeOutOfRange = "TableCollectionRangeOutOfRange";
        public const String TableCollectionRankMultiDimNotSupported = "TableCollectionRankMultiDimNotSupported";
        public const String TableCollectionWrongProxyParent = "TableCollectionWrongProxyParent";
        public const String TableInvalidParentNodeType = "TableInvalidParentNodeType";
        public const String TargetNameNotFound = "TargetNameNotFound";
        public const String TargetNameNotSupportedForStyleSetters = "TargetNameNotSupportedForStyleSetters";
        public const String Template3DValueOnly = "Template3DValueOnly";
        public const String TemplateBadDictionaryKey = "TemplateBadDictionaryKey";

        public const String TemplateCannotHaveNestedContentPresenterAndGridViewRowPresenter =
            "TemplateCannotHaveNestedContentPresenterAndGridViewRowPresenter";

        public const String TemplateChildIndexOutOfRange = "TemplateChildIndexOutOfRange";
        public const String TemplateCircularReferenceFound = "TemplateCircularReferenceFound";
        public const String TemplateContentSetTwice = "TemplateContentSetTwice";
        public const String TemplateDupName = "TemplateDupName";
        public const String TemplateFindNameInInvalidElement = "TemplateFindNameInInvalidElement";
        public const String TemplateHasNestedNameScope = "TemplateHasNestedNameScope";
        public const String TemplateInvalidBamlRecord = "TemplateInvalidBamlRecord";
        public const String TemplateInvalidRootElementTag = "TemplateInvalidRootElementTag";
        public const String TemplateKnownTagWrongLocation = "TemplateKnownTagWrongLocation";
        public const String TemplateMustBeFE = "TemplateMustBeFE";
        public const String TemplateNoMultipleRoots = "TemplateNoMultipleRoots";
        public const String TemplateNoProp = "TemplateNoProp";
        public const String TemplateNoTarget = "TemplateNoTarget";
        public const String TemplateNoTemplateBindInVisualTrigger = "TemplateNoTemplateBindInVisualTrigger";
        public const String TemplateNoTriggerTarget = "TemplateNoTriggerTarget";
        public const String TemplateNotTargetType = "TemplateNotTargetType";
        public const String TemplateTagNotSupported = "TemplateTagNotSupported";
        public const String TemplateTargetTypeMismatch = "TemplateTargetTypeMismatch";
        public const String TemplateTextNotSupported = "TemplateTextNotSupported";
        public const String TemplateUnknownProp = "TemplateUnknownProp";

        public const String TextBoxBase_CantSetIsUndoEnabledInsideChangeBlock =
            "TextBoxBase_CantSetIsUndoEnabledInsideChangeBlock";

        public const String TextBoxBase_UnmatchedEndChange = "TextBoxBase_UnmatchedEndChange";

        public const String TextBoxDecoratorMarkedAsTextBoxContentMustHaveNoContent =
            "TextBoxDecoratorMarkedAsTextBoxContentMustHaveNoContent";

        public const String TextBoxInvalidChild = "TextBoxInvalidChild";
        public const String TextBoxInvalidTextContainer = "TextBoxInvalidTextContainer";
        public const String TextBoxMinMaxLinesMismatch = "TextBoxMinMaxLinesMismatch";

        public const String TextBoxScrollViewerMarkedAsTextBoxContentMustHaveNoContent =
            "TextBoxScrollViewerMarkedAsTextBoxContentMustHaveNoContent";

        public const String TextBox_ContextMenu_Copy = "TextBox_ContextMenu_Copy";
        public const String TextBox_ContextMenu_Cut = "TextBox_ContextMenu_Cut";
        public const String TextBox_ContextMenu_Description_DBCSSpace = "TextBox_ContextMenu_Description_DBCSSpace";
        public const String TextBox_ContextMenu_Description_SBCSSpace = "TextBox_ContextMenu_Description_SBCSSpace";
        public const String TextBox_ContextMenu_IgnoreAll = "TextBox_ContextMenu_IgnoreAll";
        public const String TextBox_ContextMenu_More = "TextBox_ContextMenu_More";
        public const String TextBox_ContextMenu_NoSpellingSuggestions = "TextBox_ContextMenu_NoSpellingSuggestions";
        public const String TextBox_ContextMenu_Paste = "TextBox_ContextMenu_Paste";
        public const String TextContainerChangingReentrancyInvalid = "TextContainerChangingReentrancyInvalid";
        public const String TextContainerDoesNotContainElement = "TextContainerDoesNotContainElement";

        public const String TextContainer_UndoManagerCreatedMoreThanOnce =
            "TextContainer_UndoManagerCreatedMoreThanOnce";

        public const String TextEditorCanNotRegisterCommandHandler = "TextEditorCanNotRegisterCommandHandler";

        public const String TextEditorCopyPaste_EntryPartIsMissingInXamlPackage =
            "TextEditorCopyPaste_EntryPartIsMissingInXamlPackage";

        public const String TextEditorPropertyIsNotApplicableForTextFormatting =
            "TextEditorPropertyIsNotApplicableForTextFormatting";

        public const String TextEditorSpellerInteropHasBeenDisposed = "TextEditorSpellerInteropHasBeenDisposed";

        public const String TextEditorTypeOfParameterIsNotAppropriateForFormattingProperty =
            "TextEditorTypeOfParameterIsNotAppropriateForFormattingProperty";

        public const String TextElementCollection_CannotCopyToArrayNotSufficientMemory =
            "TextElementCollection_CannotCopyToArrayNotSufficientMemory";

        public const String TextElementCollection_IndexOutOfRange = "TextElementCollection_IndexOutOfRange";

        public const String TextElementCollection_ItemHasUnexpectedType =
            "TextElementCollection_ItemHasUnexpectedType";

        public const String TextElementCollection_NextSiblingDoesNotBelongToThisCollection =
            "TextElementCollection_NextSiblingDoesNotBelongToThisCollection";

        public const String TextElementCollection_NoEnumerator = "TextElementCollection_NoEnumerator";

        public const String TextElementCollection_PreviousSiblingDoesNotBelongToThisCollection =
            "TextElementCollection_PreviousSiblingDoesNotBelongToThisCollection";

        public const String TextElementCollection_TextElementTypeExpected =
            "TextElementCollection_TextElementTypeExpected";

        public const String TextElement_UnmatchedEndPointer = "TextElement_UnmatchedEndPointer";
        public const String TextPanelIllegalParaTypeForIAddChild = "TextPanelIllegalParaTypeForIAddChild";

        public const String TextPointer_CannotInsertTextElementBecauseItBelongsToAnotherTree =
            "TextPointer_CannotInsertTextElementBecauseItBelongsToAnotherTree";

        public const String TextPositionIsFrozen = "TextPositionIsFrozen";
        public const String TextProvider_InvalidChildElement = "TextProvider_InvalidChildElement";
        public const String TextProvider_InvalidPoint = "TextProvider_InvalidPoint";
        public const String TextProvider_TextSelectionNotSupported = "TextProvider_TextSelectionNotSupported";

        public const String TextRangeEdit_InvalidStructuralPropertyApply =
            "TextRangeEdit_InvalidStructuralPropertyApply";

        public const String TextRangeProvider_EmptyStringParameter = "TextRangeProvider_EmptyStringParameter";
        public const String TextRangeProvider_InvalidParameterValue = "TextRangeProvider_InvalidParameterValue";
        public const String TextRangeProvider_WrongTextRange = "TextRangeProvider_WrongTextRange";
        public const String TextRange_InvalidParameterValue = "TextRange_InvalidParameterValue";

        public const String TextRange_PropertyCannotBeIncrementedOrDecremented =
            "TextRange_PropertyCannotBeIncrementedOrDecremented";

        public const String TextRange_UnrecognizedStructureInDataFormat =
            "TextRange_UnrecognizedStructureInDataFormat";

        public const String TextRange_UnsupportedDataFormat = "TextRange_UnsupportedDataFormat";

        public const String TextSchema_CannotInsertContentInThisPosition =
            "TextSchema_CannotInsertContentInThisPosition";

        public const String TextSchema_CannotSplitElement = "TextSchema_CannotSplitElement";
        public const String TextSchema_ChildTypeIsInvalid = "TextSchema_ChildTypeIsInvalid";
        public const String TextSchema_IllegalElement = "TextSchema_IllegalElement";
        public const String TextSchema_IllegalHyperlinkChild = "TextSchema_IllegalHyperlinkChild";
        public const String TextSchema_TextIsNotAllowed = "TextSchema_TextIsNotAllowed";
        public const String TextSchema_TextIsNotAllowedInThisContext = "TextSchema_TextIsNotAllowedInThisContext";

        public const String TextSchema_TheChildElementBelongsToAnotherTreeAlready =
            "TextSchema_TheChildElementBelongsToAnotherTreeAlready";

        public const String TextSchema_ThisBlockUIContainerHasAChildUIElementAlready =
            "TextSchema_ThisBlockUIContainerHasAChildUIElementAlready";

        public const String TextSchema_ThisInlineUIContainerHasAChildUIElementAlready =
            "TextSchema_ThisInlineUIContainerHasAChildUIElementAlready";

        public const String TextSchema_UIElementNotAllowedInThisPosition =
            "TextSchema_UIElementNotAllowedInThisPosition";

        public const String TextSegmentsMustNotOverlap = "TextSegmentsMustNotOverlap";
        public const String TextStore_BadIMECharOffset = "TextStore_BadIMECharOffset";
        public const String TextStore_BadLockFlags = "TextStore_BadLockFlags";
        public const String TextStore_CompositionRejected = "TextStore_CompositionRejected";
        public const String TextStore_CONNECT_E_CANNOTCONNECT = "TextStore_CONNECT_E_CANNOTCONNECT";
        public const String TextStore_CONNECT_E_NOCONNECTION = "TextStore_CONNECT_E_NOCONNECTION";
        public const String TextStore_E_NOINTERFACE = "TextStore_E_NOINTERFACE";
        public const String TextStore_E_NOTIMPL = "TextStore_E_NOTIMPL";
        public const String TextStore_NoSink = "TextStore_NoSink";
        public const String TextStore_ReentrantRequestLock = "TextStore_ReentrantRequestLock";
        public const String TextStore_TS_E_FORMAT = "TextStore_TS_E_FORMAT";
        public const String TextStore_TS_E_INVALIDPOINT = "TextStore_TS_E_INVALIDPOINT";
        public const String TextStore_TS_E_NOLAYOUT = "TextStore_TS_E_NOLAYOUT";
        public const String TextStore_TS_E_READONLY = "TextStore_TS_E_READONLY";
        public const String TextViewInvalidLayout = "TextViewInvalidLayout";
        public const String ThemeDictionaryExtension_Name = "ThemeDictionaryExtension_Name";
        public const String ThemeDictionaryExtension_Source = "ThemeDictionaryExtension_Source";
        public const String ToolBar_InvalidStyle_ToolBarOverflowPanel = "ToolBar_InvalidStyle_ToolBarOverflowPanel";
        public const String ToolBar_InvalidStyle_ToolBarPanel = "ToolBar_InvalidStyle_ToolBarPanel";
        public const String ToolTipStaysOpenFalseNotAllowed = "ToolTipStaysOpenFalseNotAllowed";
        public const String ToStringFormatString_Control = "ToStringFormatString_Control";
        public const String ToStringFormatString_GridView = "ToStringFormatString_GridView";
        public const String ToStringFormatString_GridViewColumn = "ToStringFormatString_GridViewColumn";
        public const String ToStringFormatString_GridViewRowPresenter = "ToStringFormatString_GridViewRowPresenter";

        public const String ToStringFormatString_GridViewRowPresenterBase =
            "ToStringFormatString_GridViewRowPresenterBase";

        public const String ToStringFormatString_HeaderedContentControl =
            "ToStringFormatString_HeaderedContentControl";

        public const String ToStringFormatString_HeaderedItemsControl = "ToStringFormatString_HeaderedItemsControl";
        public const String ToStringFormatString_ItemsControl = "ToStringFormatString_ItemsControl";
        public const String ToStringFormatString_RangeBase = "ToStringFormatString_RangeBase";
        public const String ToStringFormatString_ToggleButton = "ToStringFormatString_ToggleButton";
        public const String Track_SameButtons = "Track_SameButtons";
        public const String TransformNotSupported = "TransformNotSupported";
        public const String TriggerActionAlreadySealed = "TriggerActionAlreadySealed";
        public const String TriggerActionMustBelongToASingleTrigger = "TriggerActionMustBelongToASingleTrigger";
        public const String TriggerOnStyleNotAllowedToHaveSource = "TriggerOnStyleNotAllowedToHaveSource";
        public const String TriggersSupportsEventTriggersOnly = "TriggersSupportsEventTriggersOnly";
        public const String TrustNotGrantedText = "TrustNotGrantedText";
        public const String TrustNotGrantedTitle = "TrustNotGrantedTitle";
        public const String TwoWayBindingNeedsPath = "TwoWayBindingNeedsPath";
        public const String TypeIdOutOfSequence = "TypeIdOutOfSequence";
        public const String TypeMustImplementIAddChild = "TypeMustImplementIAddChild";
        public const String TypeNameMustBeSpecified = "TypeNameMustBeSpecified";
        public const String TypeValueSerializerUnavailable = "TypeValueSerializerUnavailable";
        public const String UIA_OperationCannotBePerformed = "UIA_OperationCannotBePerformed";
        public const String UiLessPageFunctionNotCallingOnReturn = "UiLessPageFunctionNotCallingOnReturn";
        public const String UnableToConvertInt32 = "UnableToConvertInt32";
        public const String UnableToLocateResource = "UnableToLocateResource";
        public const String UndefinedHighlightAnchor = "UndefinedHighlightAnchor";
        public const String UndoContainerTypeMismatch = "UndoContainerTypeMismatch";
        public const String UndoManagerAlreadyAttached = "UndoManagerAlreadyAttached";
        public const String UndoNoOpenParentUnit = "UndoNoOpenParentUnit";
        public const String UndoNoOpenUnit = "UndoNoOpenUnit";
        public const String UndoNotInNormalState = "UndoNotInNormalState";
        public const String UndoServiceDisabled = "UndoServiceDisabled";
        public const String UndoUnitAlreadyOpen = "UndoUnitAlreadyOpen";
        public const String UndoUnitCantBeAddedTwice = "UndoUnitCantBeAddedTwice";
        public const String UndoUnitCantBeOpenedTwice = "UndoUnitCantBeOpenedTwice";
        public const String UndoUnitLocked = "UndoUnitLocked";
        public const String UndoUnitNotFound = "UndoUnitNotFound";
        public const String UndoUnitNotOnTopOfStack = "UndoUnitNotOnTopOfStack";
        public const String UndoUnitOpen = "UndoUnitOpen";
        public const String UnexpectedAttribute = "UnexpectedAttribute";
        public const String UnexpectedCollectionChangeAction = "UnexpectedCollectionChangeAction";
        public const String UnexpectedProperty = "UnexpectedProperty";
        public const String UnexpectedType = "UnexpectedType";
        public const String UnexpectedValueTypeForCondition = "UnexpectedValueTypeForCondition";
        public const String UnexpectedValueTypeForDataTrigger = "UnexpectedValueTypeForDataTrigger";

        public const String UnexpectedXmlNodeInXmlFixedPageInfoConstructor =
            "UnexpectedXmlNodeInXmlFixedPageInfoConstructor";

        public const String UnknownBamlRecord = "UnknownBamlRecord";
        public const String UnknownContainerFormat = "UnknownContainerFormat";
        public const String UnknownErrorText = "UnknownErrorText";
        public const String UnknownErrorTitle = "UnknownErrorTitle";
        public const String UnknownIndexType = "UnknownIndexType";
        public const String UnmatchedBracket = "UnmatchedBracket";
        public const String UnmatchedLocComment = "UnmatchedLocComment";
        public const String UnmatchedParen = "UnmatchedParen";
        public const String UnRecognizedBamlNodeType = "UnRecognizedBamlNodeType";
        public const String UnserializableKeyValue = "UnserializableKeyValue";
        public const String UnsupportedTriggerInStyle = "UnsupportedTriggerInStyle";
        public const String UnsupportedTriggerInTemplate = "UnsupportedTriggerInTemplate";
        public const String Untitled = "Untitled";
        public const String UntitledPrintJobDescription = "UntitledPrintJobDescription";
        public const String UriNotMatchWithRootType = "UriNotMatchWithRootType";
        public const String ValidationRule_UnexpectedValue = "ValidationRule_UnexpectedValue";
        public const String ValidationRule_UnknownStep = "ValidationRule_UnknownStep";
        public const String Validation_ConversionFailed = "Validation_ConversionFailed";
        public const String ValueMustBeXamlReader = "ValueMustBeXamlReader";
        public const String ValueNotBetweenInt32MinMax = "ValueNotBetweenInt32MinMax";
        public const String ValueSerializerContextUnavailable = "ValueSerializerContextUnavailable";

        public const String VirtualizedCellInfoCollection_DoesNotSupportIndexChanges =
            "VirtualizedCellInfoCollection_DoesNotSupportIndexChanges";

        public const String VirtualizedCellInfoCollection_IsReadOnly = "VirtualizedCellInfoCollection_IsReadOnly";
        public const String VirtualizedElement = "VirtualizedElement";
        public const String VisualTreeRootIsFrameworkElement = "VisualTreeRootIsFrameworkElement";

        public const String VisualTriggerSettersIncludeUnsupportedSetterType =
            "VisualTriggerSettersIncludeUnsupportedSetterType";

        public const String WebBrowserNoCastToIWebBrowser2 = "WebBrowserNoCastToIWebBrowser2";
        public const String WebBrowserOverlap = "WebBrowserOverlap";
        public const String WebRequestCreationFailed = "WebRequestCreationFailed";
        public const String WindowAlreadyClosed = "WindowAlreadyClosed";
        public const String WindowMustBeRoot = "WindowMustBeRoot";
        public const String WindowPassedShouldBeOnApplicationThread = "WindowPassedShouldBeOnApplicationThread";
        public const String WpfPayload_InvalidImageSource = "WpfPayload_InvalidImageSource";
        public const String WriteNotSupported = "WriteNotSupported";
        public const String WrongNavigateRootElement = "WrongNavigateRootElement";
        public const String WrongSelectionType = "WrongSelectionType";
        public const String XamlFilterNestedFixedPage = "XamlFilterNestedFixedPage";
        public const String XmlGlyphRunInfoIsNonGraphic = "XmlGlyphRunInfoIsNonGraphic";
        public const String XmlNodeAlreadyOwned = "XmlNodeAlreadyOwned";

        public const String XpsValidatingLoaderDiscardControlHasIncorrectType =
            "XpsValidatingLoaderDiscardControlHasIncorrectType";

        public const String XpsValidatingLoaderDuplicateReference = "XpsValidatingLoaderDuplicateReference";

        public const String XpsValidatingLoaderMoreThanOneDiscardControlInPackage =
            "XpsValidatingLoaderMoreThanOneDiscardControlInPackage";

        public const String XpsValidatingLoaderMoreThanOnePrintTicketPart =
            "XpsValidatingLoaderMoreThanOnePrintTicketPart";

        public const String XpsValidatingLoaderMoreThanOneThumbnailInPackage =
            "XpsValidatingLoaderMoreThanOneThumbnailInPackage";

        public const String XpsValidatingLoaderMoreThanOneThumbnailPart =
            "XpsValidatingLoaderMoreThanOneThumbnailPart";

        public const String XpsValidatingLoaderPrintTicketHasIncorrectType =
            "XpsValidatingLoaderPrintTicketHasIncorrectType";

        public const String XpsValidatingLoaderRestrictedFontHasIncorrectType =
            "XpsValidatingLoaderRestrictedFontHasIncorrectType";

        public const String XpsValidatingLoaderThumbnailHasIncorrectType =
            "XpsValidatingLoaderThumbnailHasIncorrectType";

        public const String XpsValidatingLoaderUnlistedResource = "XpsValidatingLoaderUnlistedResource";
        public const String XpsValidatingLoaderUnsupportedEncoding = "XpsValidatingLoaderUnsupportedEncoding";
        public const String XpsValidatingLoaderUnsupportedMimeType = "XpsValidatingLoaderUnsupportedMimeType";

        public const String XpsValidatingLoaderUnsupportedRootNamespaceUri =
            "XpsValidatingLoaderUnsupportedRootNamespaceUri";

        public const String XpsValidatingLoaderUriNotInSamePackage = "XpsValidatingLoaderUriNotInSamePackage";
        public const String Animation_ChildMustBeKeyFrame = "Animation_ChildMustBeKeyFrame";

        public const String Animation_InvalidAnimationUsingKeyFramesDuration =
            "Animation_InvalidAnimationUsingKeyFramesDuration";

        public const String Animation_InvalidBaseValue = "Animation_InvalidBaseValue";
        public const String Animation_InvalidResolvedKeyTimes = "Animation_InvalidResolvedKeyTimes";
        public const String Animation_InvalidTimeKeyTime = "Animation_InvalidTimeKeyTime";
        public const String Animation_Invalid_DefaultValue = "Animation_Invalid_DefaultValue";
        public const String Animation_NoTextChildren = "Animation_NoTextChildren";
        public const String BrowserHostingNotSupported = "BrowserHostingNotSupported";
        public const String CannotConvertStringToType = "CannotConvertStringToType";
        public const String CannotConvertType = "CannotConvertType";
        public const String CannotModifyReadOnlyContainer = "CannotModifyReadOnlyContainer";
        public const String CannotRetrievePartsOfWriteOnlyContainer = "CannotRetrievePartsOfWriteOnlyContainer";

        public const String CollectionNumberOfElementsMustBeLessOrEqualTo =
            "CollectionNumberOfElementsMustBeLessOrEqualTo";

        public const String Collection_BadType = "Collection_BadType";

        public const String Collection_CopyTo_ArrayCannotBeMultidimensional =
            "Collection_CopyTo_ArrayCannotBeMultidimensional";

        public const String Collection_CopyTo_IndexGreaterThanOrEqualToArrayLength =
            "Collection_CopyTo_IndexGreaterThanOrEqualToArrayLength";

        public const String Collection_CopyTo_NumberOfElementsExceedsArrayLength =
            "Collection_CopyTo_NumberOfElementsExceedsArrayLength";

        public const String Enumerator_VerifyContext = "Enumerator_VerifyContext";
        public const String Enum_Invalid = "Enum_Invalid";
        public const String FileFormatException = "FileFormatException";
        public const String FileFormatExceptionWithFileName = "FileFormatExceptionWithFileName";
        public const String Freezable_CantBeFrozen = "Freezable_CantBeFrozen";
        public const String InvalidPermissionStateValue = "InvalidPermissionStateValue";
        public const String InvalidPermissionType = "InvalidPermissionType";
        public const String ParameterCannotBeNegative = "ParameterCannotBeNegative";

        public const String SecurityExceptionForSettingSandboxExternalToTrue =
            "SecurityExceptionForSettingSandboxExternalToTrue";

        public const String StringEmpty = "StringEmpty";
        public const String TokenizerHelperEmptyToken = "TokenizerHelperEmptyToken";
        public const String TokenizerHelperExtraDataEncountered = "TokenizerHelperExtraDataEncountered";
        public const String TokenizerHelperMissingEndQuote = "TokenizerHelperMissingEndQuote";
        public const String TokenizerHelperPrematureStringTermination = "TokenizerHelperPrematureStringTermination";
        public const String TypeMetadataCannotChangeAfterUse = "TypeMetadataCannotChangeAfterUse";
        public const String UnexpectedParameterType = "UnexpectedParameterType";
        public const String Visual_ArgumentOutOfRange = "Visual_ArgumentOutOfRange";
        private static ResourceManager s_resourceManager;
    }
}
