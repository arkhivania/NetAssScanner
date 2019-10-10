using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Nailhang.IndexBase.PublicApi;

namespace Nailhang.Processing.PublicExtract.Processing
{
    class PublicExtractor : Base.IClassExtractor
    {
        public IEnumerable<Class> ExtractClasses(ModuleDefinition module)
        {
            foreach (var t in module.GetTypes())
            {
                if (t.IsPublic)
                {
                    var @class = new Class { Name = t.Name };
                    if (t.IsInterface)
                        @class.ClassType = ClassType.Interface;
                    else if (t.IsClass)
                        @class.ClassType = ClassType.Class;
                    else if (t.IsEnum)
                        @class.ClassType = ClassType.Enum;
                    else
                        throw new InvalidOperationException("Unknown public class");

                    var methods = new List<Method>();
                    foreach(var m in t.Methods)
                    {
                        if(m.IsPublic)
                        {
                            var constructed = new Method
                            {
                                Name = m.Name,
                                Returns = m.ReturnType.ToString(),
                                Parameters = m.Parameters.Select(q => new Parameter { Name = q.Name, Type = q.ParameterType.Name }).ToArray(),
                                GenericParameters = m.GenericParameters.Select(q => q.ToString()).ToArray()
                            };

                            methods.Add(constructed);
                        }
                    }
                    @class.Methods = methods.ToArray();
                    yield return @class;
                }
            }

            yield break;
        }
    }
}
