const TabsEnum = Object.freeze({
    Bib: 1,
    RichText: 2
});

function ExportReferencesModal(obsProjectId) {
    var self = this;

    self.ProjectId = obsProjectId;
    self.Element = ko.observable();
    self.CurrentTab = ko.observable(TabsEnum.Bib);
    self.Filename = ko.observable('');
    
    self.BibDownloadUrl = ko.pureComputed(() => {
        return ApiUrls['DownloadBibFile'] 
            + "?projectId=" + self.ProjectId()
            + "&filename=" + self.Filename();
    });

    self.BibDownloadOnClick = () => {
        $(self.Element()).modal('hide');
        return true; // Allows default click action to take place (the download)
    };

    self.Show = () => {
        // Clear out old data
        self.Filename('');
        self.CurrentTab(TabsEnum.Bib);

        // Load export results

        // Show modal
        $(self.Element()).modal('show');
    };

    self.ActivateBibTab = () => self.CurrentTab(TabsEnum.Bib);
    self.ActivateRichTextTab = () => self.CurrentTab(TabsEnum.RichText);

    self.IsLargeModal = ko.pureComputed(() => self.CurrentTab() === TabsEnum.RichText);
}