import {Pipe, PipeTransform} from "@angular/core"

@Pipe({ name: "LowerCasePipe" })
export class LowerCasePipe implements PipeTransform
{
    public transform(value: IBaseData[], args: any[]): IBaseData[]
    {
        if (!value || !value.sort) { return value; }

        return value.map((value) =>
        {
            value.name = value.name.toLowerCase();
            return value;
        }).sort((firstItem, secondItem) =>
        {
            if (firstItem.name < secondItem.name)
                return -1;
            if (firstItem.name > secondItem.name)
                return 1;
            return 0;
        });
    }
}