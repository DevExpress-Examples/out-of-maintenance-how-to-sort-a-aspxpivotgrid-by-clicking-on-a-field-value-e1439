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
using DevExpress.XtraPivotGrid.Data;
using System.Collections.Generic;

public partial class _Default : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
        ASPxPivotGrid1.FieldValueTemplate = new FieldValueTemplate(ASPxPivotGrid1);
    }
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

    void GetFieldsAndValues(ASPxPivotGrid pivotGrid, int fieldIndex, int visibleIndex, int dataIndex, PivotArea area, out PivotGridField dataField, out List<PivotGridField> fields, out List<object> values) {
        dataField = pivotGrid.GetFieldByArea(PivotArea.DataArea, dataIndex);        
        fields = new List<PivotGridField>();
        values = new List<object>();
        if (fieldIndex >= 0) {
            var clickedField = pivotGrid.Fields[fieldIndex];
            for (int i = 0; i <= clickedField.AreaIndex; i++) {
                var field = pivotGrid.GetFieldByArea(area, i);
                fields.Add(field);
                object value = pivotGrid.GetFieldValueByIndex(field, visibleIndex);
                values.Add(value);
            }
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
            .Append(c.ValueItem.VisibleIndex).Append("|")
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
