import {Observable} from "rxjs/Rx"
import {OpaqueToken} from "@angular/core"

export interface IVehicleService
{
    getVehicles(): Observable<IBaseData[]>;
    getVehicle(id: number): Observable<IBaseData>;
}

//export class IVehicleServiceToken { }

//export let IVehicleServiceToken = "IVehicleServiceToken";