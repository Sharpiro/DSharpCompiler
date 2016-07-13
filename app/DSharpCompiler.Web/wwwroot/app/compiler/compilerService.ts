import {Injectable} from "@angular/core";
import {Http, Response} from "@angular/http";
import {Observable} from "rxjs/Rx";
import {ICompilerService} from "./compiler";

@Injectable()
export class CompilerService implements ICompilerService
{
    constructor(private httpService: Http) { }

    public compilePascal(source: string): Observable<string>
    {
        var body = { source: source };
        var obs = this.httpService.post("/api/compiler/compilepascal", body)
            .catch(() =>
            {
                toastr.error("Compilation Error");
                return null;
            })
            .map((response: Response) => JSON.stringify(response.json().data.output));
        return obs;
    }

    public compileDSharp(source: string): Observable<string>
    {
        var body = { source: source };
        var obs = this.httpService.post("/api/compiler/compiledsharp", body)
            .catch(() =>
            {
                toastr.error("Compilation Error");
                return null;
            })
            .map((response: Response) => JSON.stringify(response.json().data.output));
        return obs;
    }
}