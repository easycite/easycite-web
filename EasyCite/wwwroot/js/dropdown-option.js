function DropdownOption(value, text) {
    var self = this;

    self.Value = ko.observable(value);
    self.Text = ko.observable(text);
}