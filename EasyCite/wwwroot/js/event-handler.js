
function EventHandler(objHandler) {
    var self = this;

    if (objHandler === undefined) objHandler = null;

    self.Handler = objHandler;
    self.Events = [];

    self.AddListener = function (objFor, fnCallback) {
        if (typeof fnCallback !== 'function')
            throw new Error('fnCallback must be of type function.');

        self.Events.push({ For: objFor, Callback: fnCallback });
    };

    self.RemoveListener = function (objFor) {

        for (var i = 0; i < self.Events.length; i++) {
            var event = self.Events[i];

            if (event.For === objFor) {
                self.Events.splice(i, 1);
                i--;
            }
        }

    };

    self.NotifyListeners = function () {

        var events = Array.from(self.Events);
        var results = [];

        for (var i = 0; i < events.length; i++) {
            if (events[i] !== null && typeof events[i].Callback === 'function') {

                var ret = events[i].Callback.apply(self.Handler, arguments);
                if (typeof ret !== 'undefined' && ret !== null)
                    results.push(ret);
            }
        }

        return results;
    };

}