import { Injectable } from "@angular/core";
import { Http, Response } from "@angular/http";
import { Observable } from "rxjs/Rx";
import { ICompilerService } from "./compiler";

@Injectable()
export class CompilerService implements ICompilerService {
    constructor(private httpService: Http) { }

    public compilePascal(source: string): Observable<string> {
        var body = { source: source };
        var obs = this.httpService.post("/api/compiler/compilepascal", body)
            .catch(() => {
                toastr.error("Compilation Error");
                return null;
            })
            .map((response: Response) => JSON.stringify(response.json().data.output));
        return obs;
    }

    public compileDSharp(source: string): Observable<string> {
        var body = { source: source };
        var obs = this.httpService.post("/api/compiler/compiledsharp", body)
            .catch((err) => {
                toastr.error(err._body);
                return null;
            })
            .map((response: Response) => response.json().data.output);
        return obs;
    }

    public compileCSharp(source: string): Observable<string>
    {
        var body = { source: source };
        var obs = this.httpService.post("/api/compiler/compilecsharp", body)
            .catch(() => {
                toastr.error("Compilation Error");
                return null;
            })
            .map((response: Response) => JSON.stringify(response.json().data.output));
        return obs;
    }

    public transpileCSharp(source: string): Observable<string>
    {
        var body = { source: source };
        var obs = this.httpService.post("/api/compiler/TranspileCSharp", body)
            .catch(() => {
                toastr.error("Compilation Error");
                return null;
            })
            .map((response: Response) => response.json().data.output);
        return obs;
    }

    public getSyntaxTree(source: string): Observable<string>
    {
        var body = { source: source };
        var obs = this.httpService.post("/api/compiler/GetSyntaxTree", body)
            .catch(() => {
                toastr.error("Compilation Error");
                return null;
            })
            .map((response: Response) => response.json().data.output);
        return obs;
    }
}