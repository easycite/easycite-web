function DropdownOption(value, text, helpText) {
    var self = this;

    self.Value = ko.observable(value);
    self.Text = ko.observable(text);
    self.HelpText = ko.observable(helpText);
}