function ImportReferencesMvvm() {
    const self = this;
    self.IsLoading = ko.observable(false);

    self.ModalElement = ko.observable();
    self.SearchElement = ko.observable();
    self.FileElement = ko.observable();
    self.ChooseFileLabel = ko.observable();
    
    self.SearchByNameUrl = ko.observable(ApiUrls['SearchByName']);
    self.UploadAndSearchByBibFileUrl = ko.observable(ApiUrls['UploadAndSearchByBibFile']);
    
    self.IsResultMode = ko.observable(false);
    self.SearchText = ko.observable('');
    self.SearchResults = ko.observableArray();
    
    self.Show = () => {
        // reset state information
        self.ResetToSearchMode(true);
        
        $(self.ModalElement())
            .modal('show')
            .find('.ieee-search').focus();
    };
    
    self.ResetToSearchMode = (clearSearchText) => {
        self.IsLoading(false);
        self.ChooseFileLabel('Upload a .bib file...');
        self.IsResultMode(false);
        self.FileElement().value = '';
        self.SearchResults.removeAll();
        if (clearSearchText === true) {
            self.SearchText('');
        }
    };
    
    self.UpdateWithResults = results => {
        self.IsResultMode(true);
        self.SearchResults.removeAll();
        results.map(r => new SearchResultVm(r)).forEach(function (r) {
            self.SearchResults.push(r);
        });
        self.IsLoading(false);
    };
    
    self.SubmitSearch = () => {
        self.IsLoading(true);
        $.get(self.SearchByNameUrl(), { term: self.SearchText() }).done(self.UpdateWithResults);
    };
    
    self.FileChanged = (data, e) => {
        if (e.target.files.length) {
            const file = e.target.files[0];
            const fileName = file.name;
            self.ChooseFileLabel(fileName);

            const formData = new FormData();
            formData.append('file', file);
            
            $.post({
                url: self.UploadAndSearchByBibFileUrl(),
                data: formData,
                processData: false,
                contentType: false
            }).done(self.UpdateWithResults);
        }
    };
    
    self.Import = () => {
        const importIds = self.SearchResults().filter(r => r.IsSelected());
        
        self.OnSave.NotifyListeners(importIds);
        $(self.ModalElement()).modal('hide');
    };
    
    self.OnSave = new EventHandler(self);
    
    self.ModalHidden = () => {
        // unbind any external events
    };
}

function SearchResultVm(result) {
    const self = this;
    
    self.Id = ko.observable(result.Id);
    self.Title = ko.observable(result.Title);
    self.Abstract = ko.observable('');
    
    self.IsSelected = ko.observable(false);
    self.SelectedClass = ko.pureComputed(function () {
        return self.IsSelected() ? 'active' : '';
    });
    self.SelectedBoxClass = ko.pureComputed(function () {
        return self.IsSelected() ? 'fas fa-check-square' : 'far fa-square';
    });
    
    self.ToggleSelected = () => {
        self.IsSelected(!self.IsSelected());
    }
}