function ProjectsMvvm() {
    var self = this;
    self.ThisIsLoading = ko.observable(false);
    self.IsLoading = ko.pureComputed({
        read: () => {
            return self.ThisIsLoading() === true ||
                self.Projects().reduce(
                    (accumulator, currentValue) => accumulator === true
                        || currentValue.IsLoading() === true,
                    false);
        },
        write: value => {
            self.ThisIsLoading(value);
        }
    });

    // --------- Observables --------- //
    self.Projects = ko.observableArray().extend({
        uniqueConstraint: {
            params: ["Name", "EditingName"]
        }
    });
    self.TableIsVisible = ko.pureComputed(() => {
        return self.Projects().length !== 0;
    });

    // ---------- Functions ---------- //
    self.OnNewProjectClick = () => {
        let newProject = new ProjectVm();
        newProject.IsEditing(true);
        newProject.OnDeleteEvent.AddListener(self, self.DeleteConfirmModal.Show);
        self.Projects.push(newProject);
    };

    // Project Callbacks
    self.OnSaveCallback = () => {

    };

    self.OnDeleteClick = project => {
        $.post(ApiUrls['Delete'], { projectId: project.Id() }, results => {
            if (self.HasException === true) return;
            
            self.Projects.remove(project);
        }).always(() => {
            self.IsLoading(false);
        });
        
    };

    self.DeleteConfirmModal = new ConfirmModal({
        Message: 'Are you sure you wish to delete this project?',
        AcceptButtonCss: 'btn-danger',
        OnAcceptCallback: self.OnDeleteClick
    });

    self.Load = () => {
        if (self.IsLoading() === true) return;
        self.IsLoading(true);

        $.get(ApiUrls['Load'], results => {
            if (results.HasException === true) return;

            self.Projects.removeAll();

            let projects = results.Data;
            for (let i = 0; i < projects.length; i++) {
                const project = new ProjectVm(projects[i]);

                // Add event listeners here
                project.OnDeleteEvent.AddListener(self, self.DeleteConfirmModal.Show);

                self.Projects.push(project);
            }
        });
    };

    self.Load();
}

function ProjectVm(project, obsProjects) {
    var self = this;
    self.IsLoading = ko.observable(false);

    self.Projects = obsProjects;

    self.Id = ko.observable();
    self.Name = ko.observable();
    self.NumberOfReferences = ko.observable();

    self.IsEditing = ko.observable(false);
    self.EditingName = ko.observable('').extend({
        required: true
    });

    self.Url = ko.pureComputed(() => {
        return ApiUrls['Search'] + '?projectId=' + self.Id();
    });

    self.OnEditEvent = new EventHandler(self);
    self.OnSaveEvent = new EventHandler(self);
    self.OnDeleteEvent = new EventHandler(self);

    self.IsValid = ko.pureComputed(() => {
        return self.EditingName.isValid();
    });

    self.IsSaveDisabled = ko.pureComputed(() => {
        return self.IsLoading() || self.IsValid() === false;
    });

    self.Load = projectVm => {
        self.Id(projectVm.Id);
        self.Name(projectVm.Name);
        self.NumberOfReferences(projectVm.NumberOfReferences);
    };

    self.OnClick = () => {
        if (self.Id() !== undefined)
            window.location = self.Url();
    };

    self.OnEditClick = () => {
        self.EditingName(self.Name());
        self.IsEditing(true);
        self.OnEditEvent.NotifyListeners();
    };

    self.OnSaveClick = () => {
        if (self.IsLoading() === true) return;
        self.IsLoading(true);

        // Validation
        if (self.IsValid() === false) return;

        self.IsEditing(false);

        let saveData = {
            Id: self.Id(),
            Name: self.EditingName()
        };

        $.post(ApiUrls['Save'], saveData, results => {
            if (results.HasException === true) return;

            self.Load(results.Data);

            self.OnSaveEvent.NotifyListeners(results.Exceptions);
        }).always(() => {
            self.IsLoading(false);
        });
    };

    
    self.OnDeleteClick = () => {
        self.OnDeleteEvent.NotifyListeners(self);
    };

    self.OnCancelClick = () => {
        if (self.Id() === undefined)
            self.OnDeleteEvent.NotifyListeners(self);
        self.IsEditing(false);
    };

    // Load the data
    if (project !== undefined && project !== null)
        self.Load(project);
}