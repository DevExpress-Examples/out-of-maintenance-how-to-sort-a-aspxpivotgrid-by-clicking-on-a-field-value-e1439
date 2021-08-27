<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E1439)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [Default.aspx.cs](./CS/WebSite/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
<!-- default file list end -->
# How to sort a ASPxPivotGrid by clicking on a field value
<!-- run online -->
**[[Run Online]](https://codecentral.devexpress.com/e1439)**
<!-- run online end -->


<p>The PivotGrid allows sorting by any column. To do this, right click any field value and select an appropriate field to sort by it. Sometimes it is possible to sort by any column by simply clicking on it. This example shows how to implement this behavior. You should create a template for field values to place a hyperlink (or a button) rather than a plain text.</p>


```cs
ASPxPivotGrid1.FieldValueTemplate = new FieldValueTemplate(ASPxPivotGrid1);


```


<p>The FieldValueTemplate class should implement the ITemplate interface. We've replaced standard contents of a field value in the InstantiateIn method.</p>


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
        HyperLink link = new HyperLink();
        link.Text = (string)c.Text;
        link.NavigateUrl = "#";
        link.Attributes["onclick"] = GetOnClickHandler(c);
        c.Controls.Add(link);
        bool isSortedByColumn = GetIsSortedByColumn(c);
        if(isSortedByColumn) {
            c.Controls.Add(new LiteralControl("&nbsp;*"));
        }
    }

    bool GetIsSortedByColumn(PivotGridFieldValueTemplateContainer c) {
        List<PivotGridFieldPair> sortedFields = PivotGrid.Data.VisualItems.GetSortedBySummaryFields(c.ValueItem.IsColumn, c.ValueItem.Index);
        bool isSortedByColumn = sortedFields != null && sortedFields.Count > 0;
        return isSortedByColumn;
    }

    string GetOnClickHandler(PivotGridFieldValueTemplateContainer c) {
        StringBuilder res = new StringBuilder();
        res.Append(pivotGrid.ClientInstanceName).Append(".PerformCallback('SC|");
        res.Append(GetFieldIndex(c)).Append("|")
            .Append(c.ValueItem.IsColumn).Append("|")
            .Append(c.ValueItem.VisibleIndex).Append("|")
            .Append(c.ValueItem.DataIndex);
        res.Append("');");
        return res.ToString();
    }

    int GetFieldIndex(PivotGridFieldValueTemplateContainer c) {
        return c.ValueItem.Field != null ? c.ValueItem.Field.Index : -1;
    }

    #endregion
}

```


<p>The GetOnClickHandler method returns a javascript code that will perform a ASPxPivotGrid's callback passing all required parameters to the server. On the server side, you should handle the CustomCallback event handler and apply sorting to it.</p>


```cs
    protected void ASPxPivotGrid1_CustomCallback(object sender, PivotGridCustomCallbackEventArgs e) {
        string[] args = e.Parameters.Split('|');
        if(args[0] == "SC")
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

```


<p>Sorting can be set by filling the SortBySummaryInfo structure. The BeginUpdate/EndUpdate method calls are necessary to prevent multiple data recalculations.</p>


```cs
    void SetSortByColumn(ASPxPivotGrid pivotGrid, PivotArea crossArea, PivotGridField dataField, List<PivotGridField> fields, List<object> values) {
        pivotGrid.BeginUpdate();
        List<PivotGridField> crossFields = pivotGrid.GetFieldsByArea(crossArea);
        for(int i = 0; i < crossFields.Count; i++) {
            crossFields[i].SortBySummaryInfo.Field = dataField;
            crossFields[i].SortBySummaryInfo.Conditions.Clear();
            for(int j = 0; j < values.Count; j++) {
                crossFields[i].SortBySummaryInfo.Conditions.Add(
                    new PivotGridFieldSortCondition(fields[j], values[j]));
            }
        }
        pivotGrid.EndUpdate();
    }
```



<br/>


