﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using GoodVibrations.Extensions;
using GoodVibrations.ViewModels;
using ReactiveUI;
using Xamarin.Forms;
using System.Linq;
using GoodVibrations.TemplateSelectors;
using System.Collections.Specialized;

namespace GoodVibrations.Pages
{
    public partial class MainPage
    {
        private readonly MainTemplateSelector _templateSelector;

        public MainPage()
        {
            InitializeComponent();
            this.AutoWireViewModel();

            _templateSelector = new MainTemplateSelector();

            this.WhenActivated(dispose =>
            {
                dispose(this.BindToTitle(ViewModel));

                dispose(ViewModel.MenuItems
                        .Changed
                        .Select(_ => Unit.Default)
                        .Merge(ViewModel.MenuItems.ItemChanged.Select(_ => Unit.Default))
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(_ => FillTableView()));

                dispose(this.BindToToolBarItems(ViewModel));

                dispose(ViewModel.ShowSelectedNotificator.RegisterHandler(async notificator =>
                {
                    await Navigation.PushAsync(new EditNotificatorPage(notificator.Input)).ConfigureAwait(false);
                    notificator.SetOutput(Unit.Default);
                }));

                dispose(ViewModel.ShowSelectedPhoneCallTemplate.RegisterHandler(async phoneCallTemplate =>
                {
                    await Navigation.PushAsync(new PhoneCallTemplatePage(phoneCallTemplate.Input)).ConfigureAwait(false);
                    phoneCallTemplate.SetOutput(Unit.Default);
                }));
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            FillTableView();
        }

        private void FillTableView()
        {
            var newRoot = new TableRoot();

            foreach (var section in ViewModel.MenuItems)
            {
                var newSection = new TableSection(section.Title);

                foreach (var item in section.Items)
                {
                    var template = _templateSelector.SelectTemplate(item, newSection);
                    var view = template.CreateContent() as Cell;
                    view.BindingContext = item;
                    newSection.Add(view);
                }

                newRoot.Add(newSection);
            }

            TableView.Root = newRoot;
        }
    }
}