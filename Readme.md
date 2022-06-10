<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128577852/21.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E1439)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
## Pivot Grid for Web Forms - How to Sort a PivotGrid by Clicking on a Field Value

The PivotGrid allows sorting by any column. To do this, right click any field value and select an appropriate field to sort by it. Sometimes it is possible to sort by any column by simply clicking on it. This example shows how to implement this behavior. 

Create a template for field values to place a hyperlink (or a button) rather than a plain text. Use the pivot grid's [FieldValueTemplate](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid.FieldValueTemplate) property.


```cs
ASPxPivotGrid1.FieldValueTemplate = new FieldValueTemplate(ASPxPivotGrid1);
```


The *FieldValueTemplate* class should implement the [ITemplate](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.itemplate?view=netframework-4.8) interface. Replace standard contents of a field value in the *InstantiateIn* method.


```cs
public class FieldValueTemplate : ITemplate {
    public FieldValueTemplate(ASPxPivotGrid pivotGrid) {
        this.pivotGrid = pivotGrid;
    }

    ASPxPivotGrid pivotGrid;
    protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }

    #region ITemplate Members

    public void InstantiateIn(Control container) {
        PivotGridFieldValueTemplateContainer c = (PivotGridFieldValueTemplateContainer)container;
        PivotGridFieldValueHtmlCell cell = c.CreateFieldValue();
        if (c.ValueItem.CanShowSortBySummary && !c.ValueItem.IsAnyFieldSortedByThisValue) {
            cell.Controls.AddAt(cell.Controls.IndexOf(cell.TextControl), GetHyperLink(c));
            cell.Controls.Remove(cell.TextControl);
        }
        c.Controls.Add(cell);
    }

    private Control GetHyperLink(PivotGridFieldValueTemplateContainer c) {
        HyperLink link = new HyperLink();
        link.Text = (string)c.Text;
        link.NavigateUrl = "#";
        link.Attributes["onclick"] = GetOnClickHandler(c);
        return link;
    }

    string GetOnClickHandler(PivotGridFieldValueTemplateContainer c) {
        StringBuilder res = new StringBuilder();
        res.Append(pivotGrid.ClientInstanceName).Append(".PerformCallback('SC|");
        res.Append(GetFieldIndex(c.ValueItem)).Append("|")
            .Append(c.ValueItem.IsColumn).Append("|")
            .Append(c.ValueItem.MaxLastLevelIndex).Append("|")
            .Append(c.ValueItem.DataIndex);
        res.Append("');");
        return res.ToString();
    }

    private int GetFieldIndex(PivotFieldValueItem valueItem) {
        if( valueItem == null || valueItem.Field == null) {
            return -1; // Grand Total Cell
        }
        if (valueItem.IsDataFieldItem) {
            return GetFieldIndex(valueItem.Parent); //Find the parent field of a Data Field cell
        }
        return valueItem.Field.Index;
    }
    #endregion
}
```


The *GetOnClickHandler* method returns a JavaScript code that will perform a ASPxPivotGrid's callback passing all required parameters to the server. On the server side, handle the *CustomCallback* event handler and apply sorting to it.


```cs
    protected void ASPxPivotGrid1_CustomCallback(object sender, PivotGridCustomCallbackEventArgs e) {
        string[] args = e.Parameters.Split('|');
        if (args[0] == "SC")
            HandleSortByColumnClick((ASPxPivotGrid)sender, args);
    }

    void HandleSortByColumnClick(ASPxPivotGrid pivotGrid, string[] args) {
        int fieldIndex = int.Parse(args[1]),
            visibleIndex = int.Parse(args[3]),
            dataIndex = int.Parse(args[4]);
        bool isColumn = bool.Parse(args[2]);
        PivotArea area = GetArea(isColumn),
            crossArea = GetCrossArea(isColumn);

        PivotGridField dataField;
        List<PivotGridField> fields;
        List<object> values;
        GetFieldsAndValues(pivotGrid, fieldIndex, visibleIndex, dataIndex, area,
            out dataField, out fields, out values);

        SetSortByColumn(pivotGrid, crossArea, dataField, fields, values);
    }
    
    void GetFieldsAndValues(ASPxPivotGrid pivotGrid, int fieldIndex, int visibleIndex, int dataIndex, PivotArea area, out PivotGridField dataField, out List<PivotGridField> fields, out List<object> values) {
        dataField = pivotGrid.GetFieldByArea(PivotArea.DataArea, dataIndex);        
        fields = new List<PivotGridField>();
        values = new List<object>();
        if (fieldIndex >= 0) {
            var clickedField = pivotGrid.Fields[fieldIndex];
            for (int i = 0; i <= clickedField.AreaIndex; i++) {
                var field = pivotGrid.GetFieldByArea(area, i);
                fields.Add(field);
                object value = pivotGrid.GetFieldValue(field, visibleIndex);
                values.Add(value);
            }
        }
    }

```


Sorting can be set by filling the *SortBySummaryInfo* structure. The BeginUpdate/EndUpdate method calls are necessary to prevent multiple data recalculations.


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
