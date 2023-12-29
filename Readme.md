# Pivot Grid for Web Forms - How to Sort a PivotGrid by Clicking on a Field Value

The PivotGrid allows you to sort by any column. To do this, right click any field value and select an appropriate field by which to sort. Sometimes it is possible to sort by any column by clicking on it. This example shows how to implement this behavior.

Create a template for field values to display a hyperlink (or a button) instead of plain text. To create a template, use the pivot grid's [FieldValueTemplate](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid.FieldValueTemplate) property.


```cs
ASPxPivotGrid1.FieldValueTemplate = new FieldValueTemplate(ASPxPivotGrid1);
```


The *FieldValueTemplate* class should implement the [ITemplate](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.itemplate?view=netframework-4.8) interface. Replace the standard content of a field value in the *InstantiateIn* method (see [Default.aspx.cs](./CS/Default.aspx.cs#L82-L131)/[Default.aspx.vb](./VB/Default.aspx.vb#L81-L134)).


The *GetOnClickHandler* method returns JavaScript code that performs an ASPxPivotGrid callback, passing all required parameters to the server. On the server side, handle the *CustomCallback* event handler and apply sorting to it (see [Default.aspx.cs](./CS/Default.aspx.cs#L20-L71)/[Default.aspx.vb](./VB/Default.aspx.vb#L24-L70)).

Sorting can be set by filling the *SortBySummaryInfo* structure. The BeginUpdate/EndUpdate method calls are necessary to prevent multiple data recalculations:


```cs
    void SetSortByColumn(ASPxPivotGrid pivotGrid, PivotArea crossArea, PivotGridField dataField, List<PivotGridField> fields, List<object> values) {
        pivotGrid.BeginUpdate();
        List<PivotGridField> crossFields = pivotGrid.GetFieldsByArea(crossArea);
        for (int i = 0; i < crossFields.Count; i++) {
            crossFields[i].SortBySummaryInfo.Field = dataField;
            crossFields[i].SortBySummaryInfo.Conditions.Clear();
            for (int j = 0; j < values.Count; j++) {
                crossFields[i].SortBySummaryInfo.Conditions.Add(
                    new PivotGridFieldSortCondition(fields[j], values[j]));
            }
        }
        pivotGrid.EndUpdate();
    }
```
## Files to Look At

- [Default.aspx](./CS/Default.aspx) (VB: [Default.aspx.vb](./VB/Default.aspx.vb))
- [Default.aspx.cs](./CS/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/Default.aspx.vb))
