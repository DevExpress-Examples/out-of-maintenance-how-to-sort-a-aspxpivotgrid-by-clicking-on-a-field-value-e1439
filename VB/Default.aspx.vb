Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports DevExpress.Web.ASPxPivotGrid
Imports System.Text
Imports DevExpress.XtraPivotGrid
Imports DevExpress.XtraPivotGrid.Data
Imports System.Collections.Generic

Partial Public Class _Default
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		ASPxPivotGrid1.FieldValueTemplate = New FieldValueTemplate(ASPxPivotGrid1)
	End Sub
	Protected Sub ASPxPivotGrid1_CustomCallback(ByVal sender As Object, ByVal e As PivotGridCustomCallbackEventArgs)
		Dim args() As String = e.Parameters.Split("|"c)
		If args(0) = "SC" Then
			HandleSortByColumnClick(DirectCast(sender, ASPxPivotGrid), args)
		End If
	End Sub

	Private Sub HandleSortByColumnClick(ByVal pivotGrid As ASPxPivotGrid, ByVal args() As String)
		Dim fieldIndex As Integer = Integer.Parse(args(1)), visibleIndex As Integer = Integer.Parse(args(3)), dataIndex As Integer = Integer.Parse(args(4))
		Dim isColumn As Boolean = Boolean.Parse(args(2))
		Dim area As PivotArea = GetArea(isColumn), crossArea As PivotArea = GetCrossArea(isColumn)

		Dim dataField As PivotGridField = Nothing
		Dim fields As List(Of PivotGridField) = Nothing
		Dim values As List(Of Object) = Nothing
		GetFieldsAndValues(pivotGrid, fieldIndex, visibleIndex, dataIndex, area, dataField, fields, values)

		SetSortByColumn(pivotGrid, crossArea, dataField, fields, values)
	End Sub

	Private Sub SetSortByColumn(ByVal pivotGrid As ASPxPivotGrid, ByVal crossArea As PivotArea, ByVal dataField As PivotGridField, ByVal fields As List(Of PivotGridField), ByVal values As List(Of Object))
		pivotGrid.BeginUpdate()
		Dim crossFields As List(Of PivotGridField) = pivotGrid.GetFieldsByArea(crossArea)
		For i As Integer = 0 To crossFields.Count - 1
			crossFields(i).SortBySummaryInfo.Field = dataField
			crossFields(i).SortBySummaryInfo.Conditions.Clear()
			For j As Integer = 0 To values.Count - 1
				crossFields(i).SortBySummaryInfo.Conditions.Add(New PivotGridFieldSortCondition(fields(j), values(j)))
			Next j
		Next i
		pivotGrid.EndUpdate()
	End Sub

	Private Sub GetFieldsAndValues(ByVal pivotGrid As ASPxPivotGrid, ByVal fieldIndex As Integer, ByVal visibleIndex As Integer, ByVal dataIndex As Integer, ByVal area As PivotArea, ByRef dataField As PivotGridField, ByRef fields As List(Of PivotGridField), ByRef values As List(Of Object))
		fields = New List(Of PivotGridField)()
		values = New List(Of Object)()
		dataField = pivotGrid.GetFieldByArea(PivotArea.DataArea, dataIndex)
		If fieldIndex >= 0 Then
			Dim clickedField = pivotGrid.Fields(fieldIndex)
			For i As Integer = 0 To clickedField.AreaIndex
				Dim field = pivotGrid.GetFieldByArea(area, i)
				fields.Add(field)
				Dim value As Object = pivotGrid.GetFieldValue(field, visibleIndex)
				values.Add(value)
			Next i
		End If
	End Sub

	Private Function GetCrossArea(ByVal isColumn As Boolean) As PivotArea
		Return If(isColumn, PivotArea.RowArea, PivotArea.ColumnArea)
	End Function

	Private Function GetArea(ByVal isColumn As Boolean) As PivotArea
		Return If(isColumn, PivotArea.ColumnArea, PivotArea.RowArea)
	End Function
End Class

Public Class FieldValueTemplate
	Implements ITemplate

	Public Sub New(ByVal pivotGrid As ASPxPivotGrid)
		Me.pivotGrid_Conflict = pivotGrid
	End Sub

'INSTANT VB NOTE: The field pivotGrid was renamed since Visual Basic does not allow fields to have the same name as other class members:
	Private pivotGrid_Conflict As ASPxPivotGrid
	Protected ReadOnly Property PivotGrid() As ASPxPivotGrid
		Get
			Return pivotGrid_Conflict
		End Get
	End Property

	#Region "ITemplate Members"

	Public Sub InstantiateIn(ByVal container As Control)
		Dim c As PivotGridFieldValueTemplateContainer = CType(container, PivotGridFieldValueTemplateContainer)
		Dim cell As PivotGridFieldValueHtmlCell = c.CreateFieldValue()
		If c.ValueItem.CanShowSortBySummary AndAlso Not c.ValueItem.IsAnyFieldSortedByThisValue Then
			cell.Controls.AddAt(cell.Controls.IndexOf(cell.TextControl), GetHyperLink(c))
			cell.Controls.Remove(cell.TextControl)
		End If
		c.Controls.Add(cell)
	End Sub

	Private Function GetHyperLink(ByVal c As PivotGridFieldValueTemplateContainer) As Control
		Dim link As New HyperLink()
		link.Text = CStr(c.Text)
		link.NavigateUrl = "#"
		link.Attributes("onclick") = GetOnClickHandler(c)
		Return link
	End Function

	Private Function GetOnClickHandler(ByVal c As PivotGridFieldValueTemplateContainer) As String
		Dim res As New StringBuilder()
		res.Append(pivotGrid_Conflict.ClientInstanceName).Append(".PerformCallback('SC|")
		res.Append(GetFieldIndex(c.ValueItem)).Append("|").Append(c.ValueItem.IsColumn).Append("|").Append(c.ValueItem.MaxLastLevelIndex).Append("|").Append(c.ValueItem.DataIndex)
		res.Append("');")
		Return res.ToString()
	End Function

	Private Function GetFieldIndex(ByVal valueItem As PivotFieldValueItem) As Integer
		If valueItem Is Nothing OrElse valueItem.Field Is Nothing Then
			Return -1 ' Grand Total Cell
		End If
		If valueItem.IsDataFieldItem Then
			Return GetFieldIndex(valueItem.Parent) 'Find the parent field of a Data Field cell
		End If
		Return valueItem.Field.Index
	End Function
	#End Region
End Class
