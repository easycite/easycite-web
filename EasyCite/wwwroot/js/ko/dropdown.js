function ComponentDropdownVm(params) {
    var self = this;
    
    self.ButtonText = ko.pureComputed(function () {
        const selectedOption = self.Options().find(o => o.OptionValue() === self.Value());
        return selectedOption ? selectedOption.OptionText() : params.text;
    });
    self.Value = params.value;
    self.Options = ko.observableArray([]);
    self.OptionsText = ko.observable(params.optionsText);
    self.OptionsValue = ko.observable(params.optionsValue);

    for (let i = 0; i < params.options().length; i++) {
        const option = params.options()[i];
        let obsText = option[self.OptionsText()];
        let obsValue = option[self.OptionsValue()];
        self.Options.push(new ComponentDropdownItemVm(obsText, obsValue, self.Value));
    }
}

function ComponentDropdownItemVm(obsOptionsText, obsOptionsValue, obsUpdateValue) {
    var self = this;

    self.OptionText = obsOptionsText;
    self.OptionValue = obsOptionsValue;
    self.UpdateValue = obsUpdateValue;

    self.OnClick = () => {
        self.UpdateValue(self.OptionValue());
    };
}

ko.components.register('dropdown', {
    viewModel: ComponentDropdownVm,
    template: { element: 'dropdown--template' }
});