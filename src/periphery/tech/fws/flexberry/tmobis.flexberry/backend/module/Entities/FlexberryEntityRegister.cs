namespace Tmobis.Frameworks.Flexberry.Entities;
using System;
using System.Collections.Generic;
using Tmobis.Core.Entities;
using Tmobis.Core.Modules;
using Tmobis.Core.Models;
using DataObject = ICSSoft.STORMNET.DataObject;

public class FlexberryEntityRegister(IModuleRegister modules)
: EntityRegister(modules)
{
  protected override List<Type> BaseEntityTypes
  => [typeof(IEntity), typeof(DataObject)];
}
