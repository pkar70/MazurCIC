Imports Windows.UI.Popups
''' <summary>
''' Provides application-specific behavior to supplement the default Application class.
''' </summary>
NotInheritable Class App
    Inherits Application

    ''' <summary>
    ''' Invoked when the application is launched normally by the end user.  Other entry points
    ''' will be used when the application is launched to open a specific file, to display
    ''' search results, and so forth.
    ''' </summary>
    ''' <param name="e">Details about the launch request and process.</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)

        ' Do not repeat app initialization when the Window already has content,
        ' just ensure that the window is active

        If rootFrame Is Nothing Then
            ' Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = New Frame()

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed

            ' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
            AddHandler rootFrame.Navigated, AddressOf OnNavigatedAddBackButton
            AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf OnBackButtonPressed

            If e.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: Load state from previously suspended application
            End If
            ' Place the frame in the current Window
            Window.Current.Content = rootFrame
        End If

        If e.PrelaunchActivated = False Then
            If rootFrame.Content Is Nothing Then
                ' When the navigation stack isn't restored navigate to the first page,
                ' configuring the new page by passing required information as a navigation
                ' parameter
                rootFrame.Navigate(GetType(MainPage), e.Arguments)
            End If

            ' Ensure the current window is active
            Window.Current.Activate()
        End If
    End Sub

    ''' <summary>
    ''' Invoked when Navigation to a certain page fails
    ''' </summary>
    ''' <param name="sender">The Frame which failed navigation</param>
    ''' <param name="e">Details about the navigation failure</param>
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    ''' <summary>
    ''' Invoked when application execution is being suspended.  Application state is saved
    ''' without knowing whether the application will be terminated or resumed with the contents
    ''' of memory still intact.
    ''' </summary>
    ''' <param name="sender">The source of the suspend request.</param>
    ''' <param name="e">Details about the suspend request.</param>
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: Save application state and stop any background activity
        deferral.Complete()
    End Sub
    Public Shared Async Function AskYN(sMsg As String) As Task(Of Boolean)
        Dim oDlg As MessageDialog = New MessageDialog(sMsg)

        oDlg.Commands.Add(New UICommand("Tak"))
        oDlg.Commands.Add(New UICommand("Nie"))
        oDlg.DefaultCommandIndex = 1
        oDlg.CancelCommandIndex = 1
        Dim oRes As IUICommand = Await oDlg.ShowAsync

        Return (oRes.Label = "Tak")

    End Function
    Public Shared Async Sub DialogBox(sMsg As String)
        Dim oMsg As New MessageDialog(sMsg)
        Await oMsg.ShowAsync
    End Sub

#Region "BackButton"
    ' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
    Private Sub OnNavigatedAddBackButton(sender As Object, e As NavigationEventArgs)
#If CONFIG = "Debug" Then
        ' próba wylapywania errorów gdy nic innego tego nie złapie
        Dim sDebugCatch As String = ""
        Try
#End If
        Dim oFrame As Frame = TryCast(sender, Frame)
        If oFrame Is Nothing Then Exit Sub

        Dim oNavig As Windows.UI.Core.SystemNavigationManager = Windows.UI.Core.SystemNavigationManager.GetForCurrentView

        If oFrame.CanGoBack Then
            oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible
        Else
            oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed
        End If


#If CONFIG = "Debug" Then
        Catch ex As Exception
            sDebugCatch = ex.Message
        End Try

        If sDebugCatch <> "" Then
#Disable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed
            App.DialogBox("DebugCatch in OnNavigatedAddBackButton:" & vbCrLf & sDebugCatch)
#Enable Warning BC42358
        End If
#End If

    End Sub

    Private Sub OnBackButtonPressed(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        Try
            TryCast(Window.Current.Content, Frame).GoBack()
            e.Handled = True
        Catch ex As Exception
        End Try
    End Sub
#End Region

#Region "Get/Set settings"

#Region "String"

    Public Shared Function GetSettingsString(sName As String, Optional sDefault As String = "") As String
        Dim sTmp As String

        sTmp = sDefault

        With Windows.Storage.ApplicationData.Current
            If .RoamingSettings.Values.ContainsKey(sName) Then
                sTmp = .RoamingSettings.Values(sName).ToString
            End If
            If .LocalSettings.Values.ContainsKey(sName) Then
                sTmp = .LocalSettings.Values(sName).ToString
            End If
        End With

        Return sTmp

    End Function

    Public Shared Sub SetSettingsString(sName As String, sValue As String)
        SetSettingsString(sName, sValue, False)
    End Sub

    Public Shared Sub SetSettingsString(sName As String, sValue As String, bRoam As Boolean)
        With Windows.Storage.ApplicationData.Current
            If bRoam Then .RoamingSettings.Values(sName) = sValue
            .LocalSettings.Values(sName) = sValue
        End With
    End Sub
#End Region
#Region "Int"
    Public Shared Function GetSettingsInt(sName As String, Optional iDefault As Integer = 0) As Integer
        Dim sTmp As Integer

        sTmp = iDefault

        With Windows.Storage.ApplicationData.Current
            If .RoamingSettings.Values.ContainsKey(sName) Then
                sTmp = CInt(.RoamingSettings.Values(sName).ToString)
            End If
            If .LocalSettings.Values.ContainsKey(sName) Then
                sTmp = CInt(.LocalSettings.Values(sName).ToString)
            End If
        End With

        Return sTmp

    End Function

    Public Shared Sub SetSettingsInt(sName As String, sValue As Integer)
        SetSettingsInt(sName, sValue, False)
    End Sub

    Public Shared Sub SetSettingsInt(sName As String, sValue As Integer, bRoam As Boolean)
        With Windows.Storage.ApplicationData.Current
            If bRoam Then .RoamingSettings.Values(sName) = sValue.ToString
            .LocalSettings.Values(sName) = sValue.ToString
        End With
    End Sub
#End Region
#Region "Bool"
    Public Shared Function GetSettingsBool(sName As String, Optional iDefault As Boolean = False) As Boolean
        Dim sTmp As Boolean

        sTmp = iDefault
        With Windows.Storage.ApplicationData.Current
            If .RoamingSettings.Values.ContainsKey(sName) Then
                sTmp = CBool(.RoamingSettings.Values(sName).ToString)
            End If
            If .LocalSettings.Values.ContainsKey(sName) Then
                sTmp = CBool(.LocalSettings.Values(sName).ToString)
            End If
        End With

        Return sTmp

    End Function
    Public Shared Sub SetSettingsBool(sName As String, sValue As Boolean)
        SetSettingsBool(sName, sValue, False)
    End Sub

    Public Shared Sub SetSettingsBool(sName As String, sValue As Boolean, bRoam As Boolean)
        With Windows.Storage.ApplicationData.Current
            If bRoam Then .RoamingSettings.Values(sName) = sValue.ToString
            .LocalSettings.Values(sName) = sValue.ToString
        End With
    End Sub
#End Region

#End Region


End Class
