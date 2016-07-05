import {Injectable} from "@angular/core"
import {Http} from "@angular/http"
import {Observable} from "rxjs/Rx"
import {IVehicleService} from "./vehicles"

@Injectable()
export class VehicleService implements IVehicleService
{
    constructor(private httpService: Http) { }

    public getVehicles(): Observable<IBaseData[]>
    {
        var obs = this.httpService.get("/api/vehicles/getdata")
            .map(responese => <IBaseData[]>(responese.json().data));
        return obs;
    }

    public getVehicle(id: number): Observable<IBaseData>
    {
        var promise = this.getVehicles().map(vehicles => vehicles.find(vehicle => vehicle.id === id));
        return promise;
    }
}