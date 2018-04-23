using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxPivotGrid;
using System.Text;
using DevExpress.XtraPivotGrid;
using System.Collections.Generic;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ASPxPivotGrid1.FieldValueTemplate = new FieldValueTemplate(ASPxPivotGrid1);
    }
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

    void GetFieldsAndValues(ASPxPivotGrid pivotGrid, int fieldIndex, int visibleIndex, int dataIndex, PivotArea area, out PivotGridField dataField, out List<PivotGridField> fields, out List<object> values) {
        dataField = pivotGrid.GetFieldByArea(PivotArea.DataArea, dataIndex);
        fields = pivotGrid.GetFieldsByArea(area);
        values = new List<object>(fields.Count);
        for(int i = 0; i < fields.Count; i++) {
            object value = pivotGrid.GetFieldValueByIndex(fields[i], visibleIndex);
            values.Add(value);
            if(fields[i].Index == fieldIndex) break;
        }
    }

    PivotArea GetCrossArea(bool isColumn) {
        return isColumn ? PivotArea.RowArea : PivotArea.ColumnArea;
    }

    PivotArea GetArea(bool isColumn) {
        return isColumn ? PivotArea.ColumnArea : PivotArea.RowArea;
    }
}

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
