using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

/// <summary>
/// GreiView의 Cell Merge 후 행선택을 위한 클래스
/// Download : 출처 Unknown
/// </summary>
namespace DSLibrary
{
    public class GridRowSelection
    {
        protected GridView _view;
        protected ArrayList _selection;

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public GridRowSelection()
        {
            _selection = new ArrayList();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="view"></param>
        public GridRowSelection(GridView view)
            : this()
        {
            View = view;
            _view.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
            anchor = _view.GetVisibleIndex(view.FocusedRowHandle);
        }

        #endregion

        #region 속성
        /// <summary>
        /// 
        /// </summary>
        public GridView View
        {
            get { return _view; }
            set
            {
                if (_view != value)
                {
                    Detach();
                    Attach(value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SelectedCount
        {
            get { return _selection.Count; }
        }

        #endregion

        #region 메서드
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetSelectedRow(int index)
        {
            return _selection[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public int GetSelectedIndex(object row)
        {
            return _selection.IndexOf(row);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearSelection()
        {
            _selection.Clear();
            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SelectAll()
        {
            _selection.Clear();

            for (var i = 0; i < _view.DataRowCount; i++)
                _selection.Add(_view.GetRow(i));

            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowHandle"></param>
        /// <param name="select"></param>
        public void SelectGroup(int rowHandle, bool select)
        {
            if (IsGroupRowSelected(rowHandle) && select)
                return;

            for (var i = 0; i < _view.GetChildRowCount(rowHandle); i++)
            {
                int childRowHandle = _view.GetChildRowHandle(rowHandle, i);

                if (_view.IsGroupRow(childRowHandle))
                    SelectGroup(childRowHandle, select);
                else
                    SelectRow(childRowHandle, select, false);
            }

            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowHandle"></param>
        /// <param name="select"></param>
        public void SelectRow(int rowHandle, bool select)
        {
            SelectRow(rowHandle, select, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowHandle"></param>
        public void InvertRowSelection(int rowHandle)
        {
            if (View.IsDataRow(rowHandle))
                SelectRow(rowHandle, !IsRowSelected(rowHandle));

            if (View.IsGroupRow(rowHandle))
                SelectGroup(rowHandle, !IsGroupRowSelected(rowHandle));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowHandle"></param>
        /// <returns></returns>
        public bool IsGroupRowSelected(int rowHandle)
        {
            for (var i = 0; i < _view.GetChildRowCount(rowHandle); i++)
            {
                int row = _view.GetChildRowHandle(rowHandle, i);

                if (_view.IsGroupRow(row))
                {
                    if (!IsGroupRowSelected(row))
                    {
                        return false;
                    }
                    else
                    {
                        if (!IsRowSelected(row))
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowHandle"></param>
        /// <returns></returns>
        public bool IsRowSelected(int rowHandle)
        {
            if (_view.IsGroupRow(rowHandle))
                return IsGroupRowSelected(rowHandle);

            object row = _view.GetRow(rowHandle);
            return GetSelectedIndex(row) != -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        protected virtual void Attach(GridView view)
        {
            if (view == null)
                return;

            _selection.Clear();
            this._view = view;
            view.BeginUpdate();
            try
            {
                view.Click += new EventHandler(View_Click);
                view.RowStyle += new RowStyleEventHandler(view_RowStyle);
            }
            finally
            {
                view.EndUpdate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Detach()
        {
            if (_view == null)
                return;

            _view.Click -= new EventHandler(View_Click);
            _view.RowStyle -= new RowStyleEventHandler(view_RowStyle);
            _view = null;
        }

        /// <summary>
        /// 
        /// </summary>
        void Invalidate()
        {
            _view.CloseEditor();
            _view.BeginUpdate();
            _view.EndUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowHandle"></param>
        /// <param name="select"></param>
        /// <param name="invalidate"></param>
        void SelectRow(int rowHandle, bool select, bool invalidate)
        {
            if (IsRowSelected(rowHandle) == select)
                return;

            object row = _view.GetRow(rowHandle);
            if (select)
                _selection.Add(row);
            else
                _selection.Remove(row);

            if (invalidate)
                Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startRowHandle"></param>
        /// <param name="endRowHandle"></param>
        void SelectRange(int startRowHandle, int endRowHandle)
        {

        }

        int anchor = GridControl.InvalidRowHandle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View_Click(object sender, EventArgs e)
        {
            GridHitInfo info;
            Point pt = _view.GridControl.PointToClient(Control.MousePosition);
            info = _view.CalcHitInfo(pt);

            if (info.InRow)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    _selection.Clear();
                    int startVisibleIndex = anchor;
                    int endVisibleIndex = _view.GetVisibleIndex(info.RowHandle);
                    if (startVisibleIndex > endVisibleIndex)
                    {
                        int temp = startVisibleIndex;
                        startVisibleIndex = endVisibleIndex;
                        endVisibleIndex = temp;
                    }

                    for (var i = startVisibleIndex; i <= endVisibleIndex; i++)
                    {
                        int rowHandle = _view.GetVisibleRowHandle(i);
                        SelectRow(rowHandle, true, false);
                    }

                    Invalidate();
                }
                else
                {
                    if (Control.ModifierKeys != Keys.Control)
                        _selection.Clear();

                    InvertRowSelection(info.RowHandle);
                    anchor = _view.GetVisibleIndex(info.RowHandle);
                }
            }
            if (info.InRow && _view.IsGroupRow(info.RowHandle) && info.HitTest != GridHitTest.RowGroupButton)
            {
                InvertRowSelection(info.RowHandle);
                anchor = _view.GetVisibleIndex(info.RowHandle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void view_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (IsRowSelected(e.RowHandle))
            {
                GridControl grid = (sender as GridView).GridControl;
                Color backColor = CommonSkins.GetSkin(grid.LookAndFeel.ActiveLookAndFeel).Colors.GetColor("Highlight");
                Color foreColor = CommonSkins.GetSkin(grid.LookAndFeel.ActiveLookAndFeel).Colors.GetColor("HighlightText");

                e.Appearance.BackColor = backColor;
                e.Appearance.ForeColor = foreColor;
            }
        }

        #endregion
    }
}