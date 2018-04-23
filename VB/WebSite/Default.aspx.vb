Imports Microsoft.VisualBasic
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
Imports System.Collections.Generic

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		ASPxPivotGrid1.FieldValueTemplate = New FieldValueTemplate(ASPxPivotGrid1)
	End Sub
	Protected Sub ASPxPivotGrid1_CustomCallback(ByVal sender As Object, ByVal e As PivotGridCustomCallbackEventArgs)
		Dim args() As String = e.Parameters.Split("|"c)
		If args(0) = "SC" Then
			HandleSortByColumnClick(CType(sender, ASPxPivotGrid), args)
		End If
	End Sub

	Private Sub HandleSortByColumnClick(ByVal pivotGrid As ASPxPivotGrid, ByVal args() As String)
		Dim fieldIndex As Integer = Integer.Parse(args(1)), visibleIndex As Integer = Integer.Parse(args(3)), dataIndex As Integer = Integer.Parse(args(4))
		Dim isColumn As Boolean = Boolean.Parse(args(2))
		Dim area As PivotArea = GetArea(isColumn), crossArea As PivotArea = GetCrossArea(isColumn)

		Dim dataField As PivotGridField
		Dim fields As List(Of PivotGridField)
		Dim values As List(Of Object)
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

	Private Sub GetFieldsAndValues(ByVal pivotGrid As ASPxPivotGrid, ByVal fieldIndex As Integer, ByVal visibleIndex As Integer, ByVal dataIndex As Integer, ByVal area As PivotArea, <System.Runtime.InteropServices.Out()> ByRef dataField As PivotGridField, <System.Runtime.InteropServices.Out()> ByRef fields As List(Of PivotGridField), <System.Runtime.InteropServices.Out()> ByRef values As List(Of Object))
		dataField = pivotGrid.GetFieldByArea(PivotArea.DataArea, dataIndex)
		fields = pivotGrid.GetFieldsByArea(area)
		values = New List(Of Object)(fields.Count)
		For i As Integer = 0 To fields.Count - 1
			Dim value As Object = pivotGrid.GetFieldValueByIndex(fields(i), visibleIndex)
			values.Add(value)
			If fields(i).Index = fieldIndex Then
				Exit For
			End If
		Next i
	End Sub

	Private Function GetCrossArea(ByVal isColumn As Boolean) As PivotArea
		If isColumn Then
			Return PivotArea.RowArea
		Else
			Return PivotArea.ColumnArea
		End If
	End Function

	Private Function GetArea(ByVal isColumn As Boolean) As PivotArea
		If isColumn Then
			Return PivotArea.ColumnArea
		Else
			Return PivotArea.RowArea
		End If
	End Function
End Class

Public Class FieldValueTemplate
	Implements ITemplate
	Public Sub New(ByVal pivotGrid As ASPxPivotGrid)
		Me.pivotGrid_Renamed = pivotGrid
	End Sub

	Private pivotGrid_Renamed As ASPxPivotGrid
	Protected ReadOnly Property PivotGrid() As ASPxPivotGrid
		Get
			Return pivotGrid_Renamed
		End Get
	End Property

	#Region "ITemplate Members"

	Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
		Dim c As PivotGridFieldValueTemplateContainer = CType(container, PivotGridFieldValueTemplateContainer)
		Dim link As New HyperLink()
		link.Text = CStr(c.Text)
		link.NavigateUrl = "#"
		link.Attributes("onclick") = GetOnClickHandler(c)
		c.Controls.Add(link)
		Dim isSortedByColumn As Boolean = GetIsSortedByColumn(c)
		If isSortedByColumn Then
			c.Controls.Add(New LiteralControl("&nbsp;*"))
		End If
	End Sub

	Private Function GetIsSortedByColumn(ByVal c As PivotGridFieldValueTemplateContainer) As Boolean
		Dim sortedFields As List(Of PivotGridFieldPair) = PivotGrid.Data.VisualItems.GetSortedBySummaryFields(c.ValueItem.IsColumn, c.ValueItem.Index)
		Dim isSortedByColumn As Boolean = sortedFields IsNot Nothing AndAlso sortedFields.Count > 0
		Return isSortedByColumn
	End Function

	Private Function GetOnClickHandler(ByVal c As PivotGridFieldValueTemplateContainer) As String
		Dim res As New StringBuilder()
		res.Append(pivotGrid_Renamed.ClientInstanceName).Append(".PerformCallback('SC|")
		res.Append(GetFieldIndex(c)).Append("|").Append(c.ValueItem.IsColumn).Append("|").Append(c.ValueItem.VisibleIndex).Append("|").Append(c.ValueItem.DataIndex)
		res.Append("');")
		Return res.ToString()
	End Function

	Private Function GetFieldIndex(ByVal c As PivotGridFieldValueTemplateContainer) As Integer
		If c.ValueItem.Field IsNot Nothing Then
			Return c.ValueItem.Field.Index
		Else
			Return -1
		End If
	End Function

	#End Region
End Class
