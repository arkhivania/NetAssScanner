using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailhang.IndexBase;
using Mono.Cecil;
using System.ComponentModel;

namespace Nailhang.Processing
{
    class CecilProcessor : IndexBase.Index.IIndexProcessor
    {
        public IEnumerable<IndexBase.Module> ExtractModules(string filePath)
        {
            var moduleType = typeof(Nailhang.ModuleAttribute).FullName;

            var assDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(filePath);
            foreach(var type in assDef.MainModule.GetTypes())
            {
                var modAttr = type.CustomAttributes.FirstOrDefault(w => w.AttributeType.FullName == moduleType);
                if(modAttr != null)
                {
                    yield return ExtractModule(type, assDef, modAttr);
                }
            }
        }
 
        private Module ExtractModule(TypeDefinition moduleType, AssemblyDefinition assDef, CustomAttribute modAttr)
        {
            var res = new Module();
            res.Assembly = assDef.Name.Name;
            res.Description = moduleType
                .CustomAttributes
                .Where(w => w.AttributeType.FullName == typeof(DescriptionAttribute).FullName)
                .Where(w => w.ConstructorArguments.Count > 0)
                .Select(w => w.ConstructorArguments[0].Value)
                .OfType<string>().FirstOrDefault();
            res.FullName = moduleType.FullName;
            res.Significance = (Nailhang.Significance)(int)modAttr.Properties.FirstOrDefault(w => w.Name == "Significance").Argument.Value;
            
            return res;
        }
    }
}
