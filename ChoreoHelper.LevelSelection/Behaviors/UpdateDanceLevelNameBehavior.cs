﻿using ChoreoHelper.Entities;
using ChoreoHelper.I18N;

namespace ChoreoHelper.LevelSelection.Behaviors;

public sealed class UpdateDanceLevelNameBehavior: IBehavior<LevelSelectionViewModel>
{
    public void Activate(LevelSelectionViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.Level)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                vm.Name = vm.Level switch
                {
                    DanceLevel.Undefined => Resources.Undefined,
                    DanceLevel.Bronze => Resources.Bronce,
                    DanceLevel.Silver => Resources.Silver,
                    DanceLevel.Gold => Resources.Gold,
                    DanceLevel.Advanced => Resources.Advanced,
                    _ => "?"
                };
            })
            .DisposeWith(disposables);
    }
}