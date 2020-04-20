const TabsEnum = Object.freeze({
    Bib: 1,
    RichText: 2
});

function ExportReferencesModal(obsProjectId) {
    var self = this;
    self.IsLoading = ko.observable(false);

    self.ProjectId = obsProjectId;
    self.Element = ko.observable();
    self.BibDownloadUrl = ko.pureComputed(() => {
        return ApiUrls['DownloadBibFile'] 
            + "?projectId=" + self.ProjectId();
    });
    self.RichTextCitations = ko.observableArray([]);

    self.BibDownloadOnClick = () => {
        $(self.Element()).modal('hide');
        return true; // Allows default click action to take place (the download)
    };

    self.Show = () => {
        if(self.IsLoading() === true) return;
        self.IsLoading(true);

        // Clear out old data
        self.RichTextCitations.removeAll();
        // Load export results
        $.get(ApiUrls['GetExportData'], {
            projectId: self.ProjectId()
        }, results => {
            if(results.HasProblem) return;

            for (let i = 0; i < results.Data.RichTextCitations.length; i++) {
                const citation = results.Data.RichTextCitations[i];
                self.RichTextCitations.push(citation);
            }

        }).always(() => self.IsLoading(false));
        
        $(self.Element()).modal('show');
    };

    self.ActivateBibTab = () => self.CurrentTab(TabsEnum.Bib);
    self.ActivateRichTextTab = () => self.CurrentTab(TabsEnum.RichText);

    self.CitationElement = ko.observable();
    self.CopyText = ko.observable('Copy');
    self.CopyClicked = ko.observable(false);
    self.CopyRichText = (data, event) => {
        if(self.IsLoading() === true) return;

        let copyText = '';
        for (let i = 0; i < self.RichTextCitations().length; i++)
            copyText += self.RichTextCitations()[i] + "\n\n";
        
        // This is flipping amazing
        navigator.clipboard.writeText(copyText);

        // Animation
        self.CopyText('Copied');
        self.CopyClicked(true);
        setTimeout(() => {
            self.CopyText('Copy');
            self.CopyClicked(false);
        }, 700);
    };
}