namespace JustFeature

open System


type Feature =
    | Enabled
    | Disabled
    | Rated   of rate:double
    | Custom  of Func<bool>
    | FCustom of (unit -> bool)


module FeatureModule =
    let private random = Random()

    let isEnabled = function
        | Enabled -> true
        | Disabled -> false
        | Rated rate -> random.NextDouble() < rate
        | Custom func -> func.Invoke()
        | FCustom func -> func()
    

    let run feature ifEnabled ifDisabled =
        match isEnabled feature with
        | true -> ifEnabled()
        | false -> ifDisabled()