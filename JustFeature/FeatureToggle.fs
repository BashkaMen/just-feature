module JustFeature.FeatureToggle

open System
open System.Collections.Generic
open System.Reflection
open System.Threading

type IFeatureKey = interface end
type State = Map<string, Feature>
let private state = AsyncLocal<State>()
let setState value = state.Value <- value

let private getContext() =
    state.Value
    |> box
    |> Option.ofObj
    |> Option.map (fun x -> x :?> State)
    |> Option.defaultWith (fun () -> setState Map.empty; Map.empty)
  
let inline private getKey(t: Type) = t.FullName
let inline private keyof<'a>() = getKey typeof<'a>


[<CompiledName "RegisterAllFeatures">]
let registerAllFeatures(assemblies: seq<Assembly>, featureValue) =
    let key = typeof<IFeatureKey>

    let folder state item = Map.add item featureValue state 
    
    assemblies
    |> Seq.collect (fun x -> x.GetExportedTypes())
    |> Seq.filter (key.IsAssignableFrom)
    |> Seq.map getKey
    |> Seq.fold folder Map.empty
    |> fun x -> state.Value <- x
    


[<CompiledName "ConfigureFeature">]
let configureFeature<'a when 'a :> IFeatureKey> (feature: Feature) =
    let context = getContext()
    context.Add(keyof<'a>(), feature)
    |> setState
   
    
    
[<CompiledName "GetOr">]  
let getOr<'a when 'a :> IFeatureKey>(defaultValue) =
    let context = getContext()
    context.GetValueOrDefault(keyof<'a>(), defaultValue)



[<CompiledName "Get">]
let get<'a when 'a :> IFeatureKey>() =
    let value = getContext().GetValueOrDefault(keyof<'a>())
    (box value)
    |> Option.ofObj 
    |> Option.map (fun x -> x :?> 'a)
    
