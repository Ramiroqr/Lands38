﻿namespace Lands.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Lands.Models;
    using Lands.Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    class LandsViewModel : BaseViewModel
    {
        #region Services
        private ApiService apiService;
        #endregion

        #region Attributes
        private ObservableCollection<Land> lands;
        private bool isRefreshing;
        private string filter;
        private List<Land> landList;
        #endregion

        #region Properties
        public ObservableCollection<Land> Lands
        {
            get { return this.lands; }
            set { SetValue(ref this.lands, value); }
        }

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }

        public string Filter
        {
            get { return this.filter; }
            set
            {
                SetValue(ref this.filter, value);
                this.Search();
            }
        }
        #endregion

        #region Constructors
        public LandsViewModel()
        {
            this.apiService = new ApiService();
            this.LoadLands();
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get { return new RelayCommand(LoadLands); }
        }

        public ICommand SearchCommand
        {
            get { return new RelayCommand(Search); }
        }
        #endregion

        #region Methods
        private async void LoadLands()
        {
            this.IsRefreshing = true;

            var connection = await this.apiService.CheckConnection();

            if(!connection.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "connection.Message",
                    "Accept");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }

            var response = await this.apiService.GetList<Land>(
                "http://restcountries.eu",
                "/rest",
                "/v2/all");

            if (!response.IsSuccess)
            {
                this.IsRefreshing = true;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "response.Message",
                    "Accept");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }

            this.IsRefreshing = false;
            this.landList = (List<Land>)response.Result;
            this.Lands = new ObservableCollection<Land>(landList);
        }

        private void Search()
        {
            if(string.IsNullOrEmpty(this.Filter))
            {
                this.Lands = new ObservableCollection<Land>(this.landList);
            }
            else
            {
                this.Lands = new ObservableCollection<Land>(
                    this.landList.Where(
                        l => l.Name.ToLower().Contains(this.Filter.ToLower()) ||
                        l.Capital.ToLower().Contains(this.Filter.ToLower())));
            }

        }
        #endregion
    }
}
