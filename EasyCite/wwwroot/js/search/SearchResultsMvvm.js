function SearchResultsMvvm(projectId) {
    var self = this;
    self.IsLoading = ko.observable(false);

    // --------- Observables --------- //
    self.ProjectId = ko.observable(projectId);
    self.References = ko.observableArray();
    self.SearchResults = ko.observableArray();
    self.PageNumber = ko.observable(0);
    self.PageNumberDisplay = ko.pureComputed(() => self.PageNumber() + 1);
    self.ItemsPerPage = ko.observable(10); // TODO: make this configurable
    self.NumberOfPages = ko.observable(1);

    self.SearchTags = ko.observable('');
    self.SearchTags.subscribe(() => {
        self.IsOutOfSync(true);
    });

    self.IsOutOfSync = ko.observable(false);

    self.PreviousIsDisabled = ko.pureComputed(() => self.PageNumber() === 0);
    self.NextIsDisabled = ko.pureComputed(() => self.PageNumberDisplay() === self.NumberOfPages());

    // ------------ Urls ------------- //
    self.LoadUrl = ko.observable(ApiUrls['GetReferences']);
    self.SaveUrl = ko.observable(ApiUrls['Search']);
    self.AddReferenceUrl = ko.observable(ApiUrls['AddReference']);
    self.RemoveReferenceUrl = ko.observable(ApiUrls['RemoveReference']);

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

        $.post(self.AddReferenceUrl(), {
            projectId: self.ProjectId(),
            documentId: result.Id(),
        }).done(results => {
            self.LoadReferences(results.Data);
        }).always(() => {
            self.IsOutOfSync(true);
            self.IsLoading(false);
        });
    };

    self.OnReferenceRemoveCallback = function(reference) {
        if (self.IsLoading() === true) return;
        self.IsLoading(true);

        $.post(self.RemoveReferenceUrl(), {
            projectId: self.ProjectId(),
            documentId: reference.Id(),
        }).done(results => {
            self.LoadReferences(results.Data);
        }).always(() => {
            self.IsOutOfSync(true);
            self.IsLoading(false);
        });
    };

    self.Load = () => {
        if (self.IsLoading() === true) return;
        self.IsLoading(true);

        return $.get(self.LoadUrl(), {
            projectId: 1
        }).then(results => {
            self.LoadReferences(results.Data);
        }).always(() => {
            self.IsLoading(false);
        });
    };

    self.Search = () => {
        if (self.IsLoading() === true) return;
        self.IsLoading(true);

        var searchData = {
            PageNumber: self.PageNumber(),
            ItemsPerPage: self.ItemsPerPage(),
            SearchByIds: self.References().map(element => element.Id()),
            SearchTags: self.SearchTags().trim().split(',').map(s => s.trim()).filter(s => s)
        };

        return $.post(self.SaveUrl(), searchData).then(results => {
            var data = results.Data;

            self.NumberOfPages(data.NumberOfPages);

            self.SearchResults.removeAll();

            data.Results.map(d => new ResultVm(d)).forEach(function (result) {
                result.OnAddEvent.AddListener(self, self.OnReferenceAddCallback);
                self.SearchResults.push(result);
            });
        }).always(() => {
            self.IsLoading(false);
        });
    };

    self.SyncReferences = () => {
        self.Search().done(() => {
            self.PageNumber(0);
            self.IsOutOfSync(false);
        });
    };

    self.SearchPrevious = () => {
        let oldPage = self.PageNumber();
        self.PageNumber(Math.max(0, oldPage - 1));
        
        if(self.PageNumber() !== oldPage)
            self.Search();
    };

    self.SearchNext = () => {
        let oldPage = self.PageNumber();
        self.PageNumber(Math.min(self.NumberOfPages() - 1, oldPage + 1));
        
        if(self.PageNumber() !== oldPage)
            self.Search();
    };

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
    
    self.IsExpanded = ko.observable(false);
    self.ExpandIconClass = ko.pureComputed(() => self.IsExpanded() ? 'fa-chevron-up' : 'fa-chevron-down');

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

    self.IsAdded = ko.observable(false);
    self.AddButtonText = ko.pureComputed(() => self.IsAdded() ? 'Added' : 'Add');

    self.IsExpanded = ko.observable(false);
    self.ExpandIconClass = ko.pureComputed(() => self.IsExpanded() ? 'fa-chevron-up' : 'fa-chevron-down');

    // ----------- Events ------------ //
    self.OnAddEvent = new EventHandler(self);

    // ---------- Functions ---------- //
    self.OnAddClick = () => {
        self.IsAdded(true);
        self.OnAddEvent.NotifyListeners(self);
    };

    self.ToggleExpanded = () => {
        self.IsExpanded(!self.IsExpanded());
    };
}