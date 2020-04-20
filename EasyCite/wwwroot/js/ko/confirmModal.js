function ConfirmModal(config) {
    var self = this;
    
    self.TemplateName = config.TemplateName || 'confirm-modal--template';
    self.Title = config.Title || 'Confirm Action';
    self.Message = config.Message || 'Are you sure you wish to perform this action?';
    
    self.AcceptButtonText = config.AcceptButtonText || 'Yes';
    self.RejectButtonText = config.RejectButtonText || 'No';
    self.AcceptButtonCss = config.AcceptButtonCss || 'btn-primary';
    self.RejectButtonCss = config.RejectButtonCss || 'btn-outline-dark';
    
    self.OnAcceptCallback = config.OnAcceptCallback || function() {};
    self.AcceptParams = config.AcceptParams;
    
    self.OnRejectCallback = config.OnRejectCallback || function() {};
    self.OnHideCallback = config.OnHideCallback || function() {};
    
    self.Element = ko.observable();
    self.Element.subscribe(() => {
        $(self.Element()).on('hidden.bs.modal', self.OnHideCallback);
    });

    self.Params = ko.observable();
    self.Show = params => {
        $(self.Element()).modal('show');
        self.Params(params);
    };

    self.Hide = () => {
        $(self.Element()).modal('hide');
    };

    self.OnAccept = () => {
        self.OnAcceptCallback(self.Params());
        self.Hide();
    };

    self.OnReject = () => {
        self.OnRejectCallback(self.Params());
        self.Hide();
    };
}