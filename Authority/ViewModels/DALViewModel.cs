using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Windows;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.DAL.ViewModels
{
    [Export(typeof(DALViewModel))]
    public class DALViewModel : BindableBase
    {
        ObservableCollection<CUser> m_ListUser = new ObservableCollection<CUser>();
        ObservableCollection<CUser> m_ListAuthUser = new ObservableCollection<CUser>();
        private ObservableCollection<CFunction> function = new ObservableCollection<CFunction>();
        private ObservableCollection<CUser> userList = new ObservableCollection<CUser>();
        private ObservableCollection<CUser> authoUserList = new ObservableCollection<CUser>();
        private CUser selectedUser;
        private CUser selectUser = new CUser();
        private CFunction selectfunc;
        private CFunction m_CurrentFunction;
        private IXmlCompareUserControl m_UserControl;
        private IRegionManager _regionManager;
        public string Toimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\greed-right-arrow.ico";
        public string Fromimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\red-left-arrow.ico";
        public string Refreshimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\refresh.ico";
        public string Cancelimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\cancel.ico";
        public string Okimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\OK.ico";
        public CFunction SelectFunc
        {
            get { return selectfunc; }
            set {
                selectfunc = value;
                m_CurrentFunction = null;
                if(selectfunc != null)
                {
                    var selecteditem = selectfunc;
                    m_CurrentFunction = selecteditem;
                    m_ListAuthUser = new ObservableCollection<CUser>(m_UserControl.GetFunctionUsers(m_CurrentFunction.Name, true));
                    m_ListUser = new ObservableCollection<CUser>(m_UserControl.GetFunctionUsers(m_CurrentFunction.Name, false));
                    ReBindAuthoData(m_ListAuthUser);
                    ReBindData(m_ListUser);
                }
                RaisePropertyChanged("SelectFunc");
            }
        }
        public CUser SelectedUser
        {
            get { return selectedUser; }
            set {
                selectedUser = value;
                RaisePropertyChanged("SelectedUser");
            }
        }
        public CUser SelectUser
        {
            get { return selectUser; }
            set { selectUser = value; RaisePropertyChanged("SelectUser"); }
        }
        public ObservableCollection<CFunction> Function
        {
            get { return function; }
            set { function = value;RaisePropertyChanged("Function"); }
        }
        public ObservableCollection<CUser> UserList
        {
            get { return userList; }
            set { userList = value; RaisePropertyChanged("UserList"); }
        }
        public ObservableCollection<CUser> AuthoUserList
        {
            get { return authoUserList; }
            set { authoUserList = value;RaisePropertyChanged("AuthoUserList"); }
        }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand ToCommand { get; set; }
        public DelegateCommand FromCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand OkCommand { get; set; }
        public void CancelExecute()
        {
           _regionManager.RequestNavigate("ContentRegion", "CoreView");
        }
        public void ToCommandExecute()
        {
            if (SelectedUser != null)
            {
                var user = SelectedUser;
                m_ListUser.Remove(user);
                m_ListAuthUser.Add(user);
                ReBindAuthoData(m_ListAuthUser);
                ReBindData(m_ListUser);
            }
        }
        public void FromCommandExecute()
        {
            if (SelectUser != null)
            {
                var user = SelectUser;
                m_ListAuthUser.Remove(user);
                m_ListUser.Add(user);
                ReBindAuthoData(m_ListAuthUser);
                ReBindData(m_ListUser);
            }
        }
        public void RefreshExecute()
        {
            m_CurrentFunction = null;
        }
        public void OkCommandExecute()
        {
            m_UserControl.SetFunctionUsers(m_CurrentFunction.Name,m_ListAuthUser.ToList());
            m_CurrentFunction = null;
            if(SelectedUser != null)
            {
                m_CurrentFunction = SelectFunc;
                //get users
                m_ListAuthUser = new ObservableCollection<CUser>(m_UserControl.GetFunctionUsers(m_CurrentFunction.Name, true));
                m_ListUser = new ObservableCollection<CUser>(m_UserControl.GetFunctionUsers(m_CurrentFunction.Name, false));
                //bind data
                ReBindAuthoData(m_ListAuthUser);
                ReBindData(m_ListUser);
            }
            MessageBox.Show("Setting has been submitted.");
        }
        private void ReBindData(ObservableCollection<CUser> dataSource)
        {
            if (dataSource.Count > 0)
            {
                UserList = null;
                UserList = dataSource;
            }
            else
            {
                UserList = null;
            }
            return;
        }
        private void ReBindAuthoData(ObservableCollection<CUser> dataSource)
        {
            if (dataSource.Count > 0)
            {
                AuthoUserList = null;
                AuthoUserList = dataSource;
            }
            else
            {
                AuthoUserList = null;
            }
            return;
        }
        [ImportingConstructor]
        public DALViewModel(IRegionManager regionManager, IXmlCompareUserControl UserControl)
        {
            _regionManager = regionManager;
            m_UserControl = UserControl;
            CancelCommand = new DelegateCommand(CancelExecute);
            ToCommand = new DelegateCommand(ToCommandExecute);
            FromCommand = new DelegateCommand(FromCommandExecute);
            OkCommand = new DelegateCommand(OkCommandExecute);
            List<CFunction> lstFunc = m_UserControl.GetFunctions();
            foreach (CFunction tmp in lstFunc)
            {
                Function.Add(tmp);
            }
            m_CurrentFunction = null;
        }
    }
}
