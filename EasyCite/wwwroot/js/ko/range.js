function ComponentRangeVm(params) {
    var self = this;

    self.Min = params.config().min || ko.observable(0);
    self.Max = params.config().max || ko.observable(10);
    self.Step = params.config().step || ko.observable(1);

    self.Options = params.options;
    for (let i = 0; i < params.options.length; i++) {
        const option = params.options[i];
        self.Options.push(option);
    }

    self.OptionsHelpText = ko.observable(params.optionsHelpText || 'HelpText');
    self.OptionsText = ko.observable(params.optionsText || 'Text');
    self.OptionsValue = ko.observable(params.optionsValue || 'Value');
    self.Value = params.value;
    self.Text = ko.pureComputed(() => {
        let option = self.Options().find(opt => opt.Value() == self.Value());
        return ko.unwrap(option[self.OptionsText()]);
    });

    self.HelpText = ko.computed(() => {
        let option = self.Options().find(opt => opt.Value() == self.Value());
        return ko.unwrap(option[self.OptionsHelpText()]);
    });

}

ko.components.register('range', {
    viewModel: ComponentRangeVm,
    template: { element: 'range--template' }
});