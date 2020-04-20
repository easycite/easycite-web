function SearchResultsMvvm(results) {
    var self = this;
    self.IsLoading = ko.observable(false);

    // --------- Observables --------- //
    self.ProjectId = ko.observable(results.Data.ProjectId);
    
    // References
    self.References = ko.observableArray([]);
    self.HasReferences = ko.pureComputed(() => self.References().length > 0);
    for (let i = 0; i < results.Data.References.length; i++) {
        const reference = results.Data.References[i];
        self.References.push(new ReferenceVm(reference));
    }

    self.SearchResults = ko.observableArray();
    self.NotHasSearchResults = ko.pureComputed(() => self.SearchResults().length <= 0);
    self.PageNumber = ko.observable(0);
    self.PageNumberDisplay = ko.pureComputed(() => self.PageNumber() + 1);
    self.ItemsPerPage = ko.observable(10); // TODO: make this configurable
    self.NumberOfPages = ko.observable(1);

    // Export Modal
    self.ExportReferencesModal = new ExportReferencesModal(self.ProjectId);
    self.ShowExportModal = () => {
        if(self.HasReferences() === true)
            self.ExportReferencesModal.Show();
    };

    // Search Depth
    self.SearchDepthOptions = ko.observableArray();
    for (let i = 0; i < results.Data.SearchDepths.length; i++) {
        const searchDepth = results.Data.SearchDepths[i];
        self.SearchDepthOptions.push(new DropdownOption(searchDepth.Value, searchDepth.Text, searchDepth.HelpText));
    }

    self.SearchDepth = ko.observable(results.Data.DefaultSearchDepth);
    self.SearchDepth.subscribe(() => {
       self.IsOutOfSync(true); 
    });
    self.SearchDepthConfig = ko.observable({
        max: self.SearchDepthOptions().reduce(
            (acc, cur) => Math.max(acc, cur.Value())
        , 0)
    });

    // Search Type
    self.SearchTypes = ko.observableArray();
    for (let i = 0; i < results.Data.SortOptions.length; i++) {
        const option = results.Data.SortOptions[i];
        self.SearchTypes.push(new DropdownOption(option.Value, option.Text));
    }

    self.SelectedSearchType = ko.observable(results.Data.DefaultSortOption);
    self.SelectedSearchType.subscribe(() => {
        self.IsOutOfSync(true);
    });

    // Search Tags
    self.SearchTags = ko.observable('');
    self.SearchTags.subscribe(() => {
        self.IsOutOfSync(true);
    });

    self.IsOutOfSync = ko.observable(false);

    self.PreviousIsDisabled = ko.pureComputed(() => self.PageNumber() === 0);
    self.NextIsDisabled = ko.pureComputed(() => self.PageNumberDisplay() >= self.NumberOfPages());

    // ------------ Urls ------------- //
    self.LoadUrl = ko.observable(ApiUrls['GetReferences']);
    self.SaveUrl = ko.observable(ApiUrls['Search']);
    self.AddReferenceUrl = ko.observable(ApiUrls['AddReference']);
    self.RemoveReferenceUrl = ko.observable(ApiUrls['RemoveReference']);
    self.PendingReferencesStatusUrl = ko.observable(ApiUrls['PendingReferencesStatus']);
    self.HideResultUrl = ko.observable(ApiUrls['HideResult']);
    self.AutoCompleteKeywordsUrl = ko.observable(ApiUrls['AutoCompleteKeywords']);

    // ---------- Functions ---------- //
    self.LoadReferences = function (references) {        
        self.References.removeAll();

        references.map(d => new ReferenceVm(d)).forEach(function (reference) {
            reference.OnRemoveEvent.AddListener(self, self.OnReferenceRemoveCallback);
            self.References.push(reference);
        });
    };

    self.OnReferenceAddCallback = function(result) {
        if (self.IsLoading() === true) return;
        self.IsLoading(true);

        self.AddReferences([ result ]).always(() => {
            self.IsOutOfSync(true);
            self.IsLoading(false);
        });
    };

    self.OnReferenceRemoveCallback = function(reference) {
        if (self.IsLoading() === true) return;

        $.post(self.RemoveReferenceUrl(), {
            projectId: self.ProjectId(),
            documentId: reference.Id(),
        }).done(results => {
            if (results.Data === true) {
                self.References.remove(reference);
            }
        }).always(() => {
            self.IsOutOfSync(true);
        });
    };

    self.Load = () => {
        if (self.IsLoading() === true) return;
        self.IsLoading(true);

        return $.get(self.LoadUrl(), {
            projectId: self.ProjectId()
        }).then(results => {
            self.LoadReferences(results.Data);
            self.IsOutOfSync(false);
        }).always(() => {
            self.IsLoading(false);
        });
    };

    self.Search = pageNumber => {
        if(pageNumber === undefined) {
            if (self.IsLoading() === true) return;
            self.IsLoading(true);
        }

        const tags = self.SearchTags().trim().split(',').map(s => s.trim()).filter(s => s);
        const quotePattern = /^(["'])(.*)\1$/;
        
        const allTags = tags.map(s => quotePattern.exec(s)).filter(s => s).map(s => s[2]);
        const anyTags = tags.filter(s => !quotePattern.test(s));

        var data = {
            projectId: self.ProjectId(),
            searchData: {
                PageNumber: self.PageNumber(),
                ItemsPerPage: self.ItemsPerPage(),
                SearchByIds: self.References().map(element => element.Id()),
                AllTags: allTags,
                AnyTags: anyTags,
                ForceNoCache: self.IsOutOfSync(),
                SearchSortType: self.SelectedSearchType(),
                SearchDepth: self.SearchDepth()
            }
        };
        return $.post(self.SaveUrl(), data).then(results => {
            var data = results.Data;

            self.NumberOfPages(data.NumberOfPages);

            self.SearchResults.removeAll();

            data.Results.map(d => new ResultVm(d)).forEach(function (result) {
                result.OnAddEvent.AddListener(self, self.OnReferenceAddCallback);
                result.OnHideEvent.AddListener(self, self.ConfirmHideReferenceModal.Show);
                self.SearchResults.push(result);
            });
            self.IsOutOfSync(false);
        }).always(() => {
            self.IsLoading(false);
        });
    };
    
    self.AddReferences = references => {

        return $.post(self.AddReferenceUrl(), {
            projectId: self.ProjectId(),
            documentIds: references.map(r => r.Id())
        }).then(results => {
            const imported = results.Data;
            
            if (imported.length) {
                references.filter(r => imported.includes(r.Id())).map(r => new ReferenceVm({
                    Id: r.Id(),
                    Title: r.Title(),
                    Abstract: r.Abstract(),
                    IsPending: true
                })).forEach(function (newRef) {
                    newRef.OnRemoveEvent.AddListener(self, self.OnReferenceRemoveCallback);
                    self.References.push(newRef);
                });
            }
        });
    };

    let pendingReferencesDeferred;
    self.CheckPendingReferences = () => {
        if (pendingReferencesDeferred && pendingReferencesDeferred.state() === 'pending')
            return pendingReferencesDeferred;
        
        const pendingReferences = self.References().filter(r => r.IsPending());
        
        if (!pendingReferences.length)
            return $.Deferred().resolve().promise();

        const ids = pendingReferences.map(r => r.Id());
        pendingReferencesDeferred = $.post(self.PendingReferencesStatusUrl(), {
            projectId: self.ProjectId(),
            documentIds: ids
        }).then(function (results) {
            const completed = results.Data.map(r => new ReferenceVm(r));

            completed.forEach(function (newRef) {
                const matchingOldRef = pendingReferences.find(p => p.Id() === newRef.Id());
                if (matchingOldRef) {
                    newRef.OnRemoveEvent.AddListener(self, self.OnReferenceRemoveCallback);
                    self.References.replace(matchingOldRef, newRef);
                }
            });
        });
        
        return pendingReferencesDeferred;
    };

    self.SyncReferences = () => {
        self.Search().done(() => {
            self.PageNumber(0);
        });
    };

    self.SearchPrevious = () => {
        let oldPage = self.PageNumber();
        self.PageNumber(Math.max(0, oldPage - 1));
        
        if(self.PageNumber() !== oldPage)
            self.Search(self.PageNumber());
    };

    self.SearchNext = () => {
        let oldPage = self.PageNumber();
        self.PageNumber(Math.min(self.NumberOfPages() - 1, oldPage + 1));
        
        if(self.PageNumber() !== oldPage)
            self.Search(self.PageNumber());
    };

    self.ImportReferencesModal = new ImportReferencesMvvm();
    self.ImportReferencesModal.OnSave.AddListener(self, function (importReferences) {
        self.AddReferences(importReferences).done(function () {
            self.IsOutOfSync(true);
        });
    });

    self.OnHideResultCallback = result => {
        return $.post(self.HideResultUrl(), {
            projectId: self.ProjectId(),
            documentId: result.Id()
        }).then(function (results) {
            if (results.Data === true) {
                self.SearchResults.remove(result);
                return self.Search();
            }
        });
    };

    // Confirm Hide modal
    self.ConfirmHideReferenceModal = new ConfirmModal({
        Message: 'Are you sure you wish to hide this reference?',
        AcceptButtonCss: 'btn-danger',
        OnAcceptCallback: self.OnHideResultCallback
    });

    self.Load().then(() => {
        return self.Search();
    }).done(() => {
        // done stuff
    });
}

function ReferenceVm(reference) {
    var self = this;

    // --------- Observables --------- //
    self.Id = ko.observable(reference.Id);
    self.Title = ko.observable(reference.Title);
    self.Abstract = ko.observable(reference.Abstract);
    self.IsPending = ko.observable(reference.IsPending);
    
    self.TitleDisplay = ko.pureComputed(() => {
        if (!self.Title() && self.IsPending())
            return 'Importing reference...';
        
        return self.Title();
    });
    self.PendingTitleClass = ko.pureComputed(() => {
        return self.IsPending() ? 'font-italic text-muted' : '';
    });
    self.Tooltip = ko.pureComputed(() => {
        return self.IsPending() ? 'Importing...' : '';
    });
    
    self.IsExpanded = ko.observable(false);
    self.ExpandIconClass = ko.pureComputed(() => {
        if (!self.Abstract())
            return 'fa-chevron-up invisible';
        return self.IsExpanded() ? 'fa-chevron-up' : 'fa-chevron-down';
    });

    // ----------- Events ------------ //
    self.OnRemoveEvent = new EventHandler(self);

    // ---------- Functions ---------- //
    self.OnRemoveClick = () => {
        self.OnRemoveEvent.NotifyListeners(self);
    };

    self.ToggleExpanded = () => {
        self.IsExpanded(!self.IsExpanded());
    };
}

function ResultVm(result) {
    var self = this;

    // --------- Observables --------- //
    self.Id = ko.observable(result.Id);
    self.Title = ko.observable(result.Title);
    self.PublishDate = ko.observable(result.PublishDate);
    self.AuthorName = ko.observable(result.AuthorName);
    self.Conference = ko.observable(result.Conference);
    self.Abstract = ko.observable(result.Abstract);
    self.Url = ko.pureComputed(() => 'https://ieeexplore.ieee.org/document/' + self.Id());

    self.IsAdded = ko.observable(false);
    self.AddButtonText = ko.pureComputed(() => self.IsAdded() ? 'Added' : 'Add');

    self.IsExpanded = ko.observable(false);
    self.ExpandIconClass = ko.pureComputed(() => self.IsExpanded() ? 'fa-chevron-up' : 'fa-chevron-down');

    // ----------- Events ------------ //
    self.OnAddEvent = new EventHandler(self);
    self.OnHideEvent = new EventHandler(self);

    // ---------- Functions ---------- //
    self.OnAddClick = () => {
        self.IsAdded(true);
        self.OnAddEvent.NotifyListeners(self);
    };
    
    self.OnHideClick = () => {
        self.OnHideEvent.NotifyListeners(self);
    };

    self.ToggleExpanded = () => {
        self.IsExpanded(!self.IsExpanded());
    };
}

ko.bindingHandlers.keywordAutoComplete = {
    init: (element, valueAccessor, allBindings, viewModel) => {
        let split = function (val) {
            return val.split(/,\s*/);
        };
        let extractLast = function (term) {
            return split(term).pop();
        };
        
        const textObservable = valueAccessor();

        $(element).on('keydown', function (e) {
            if (e.keyCode === $.ui.keyCode.TAB && $(this).autocomplete('instance').menu.active) {
                e.preventDefault();
            }
        }).autocomplete({
            source: function (req, rsp) {
                const term = extractLast(req.term).trim().replace(/['"]/g, '');
                
                if (!term)
                    return;
                
                $.get(viewModel.AutoCompleteKeywordsUrl(), {
                    term: extractLast(req.term).trim().replace(/['"]/g, ''),
                    resultsCount: 10
                }).done(function (data) {
                    rsp(data.Data);
                });
            },
            search: function () {
                const term = extractLast(this.value);
                if (!term.length) {
                    return false;
                }
            },
            focus: function () {
                return false;
            },
            select: function (event, ui) {
                const terms = split(this.value);
                const popped = terms.pop().trim().substr(0, 1);
                if (popped === '"' || popped === '\'') {
                    terms.push(popped + ui.item.value + popped);
                }
                else {
                    terms.push(ui.item.value);
                }
                terms.push('');
                this.value = terms.join(', ');
                textObservable(this.value);
                return false;
            }
        });
    }
};
