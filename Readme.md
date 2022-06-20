<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E1439)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# Pivot Grid for Web Forms - How to Sort a PivotGrid by Clicking on a Field Value

The PivotGrid allows sorting by any column. To do this, right click any field value and select an appropriate field to sort by it. Sometimes it is possible to sort by any column by simply clicking on it. This example shows how to implement this behavior. 

Create a template for field values to place a hyperlink (or a button) rather than a plain text. Use the pivot grid's [FieldValueTemplate](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid.FieldValueTemplate) property.


```cs
ASPxPivotGrid1.FieldValueTemplate = new FieldValueTemplate(ASPxPivotGrid1);
```


The *FieldValueTemplate* class should implement the [ITemplate](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.itemplate?view=netframework-4.8) interface. Replace standard contents of a field value in the *InstantiateIn* method (see [Default.aspx.cs](./CS/Default.aspx.cs#L82-L131)/[Default.aspx.vb](./VB/Default.aspx.vb#L81-L134)).


The *GetOnClickHandler* method returns a JavaScript code that will perform a ASPxPivotGrid's callback passing all required parameters to the server. On the server side, handle the *CustomCallback* event handler and apply sorting to it (see [Default.aspx.cs](./CS/Default.aspx.cs#L20-L71)/[Default.aspx.vb](./VB/Default.aspx.vb#L24-L70)).

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
