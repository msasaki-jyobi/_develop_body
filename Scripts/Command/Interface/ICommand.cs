using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace develop_body
{
    // IHealth �C���^�[�t�F�[�X
    public interface ICommand
    {
        void CommandSettings();
        void CommandPlay();

        //int CurrentHealth { get; }
        //int MaxHealth { get; }
        //void TakeDamage(DamageValue damageValue = null);
        //void Heal(float amount);

    }

}
