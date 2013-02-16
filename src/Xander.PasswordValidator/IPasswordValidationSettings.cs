﻿namespace Xander.PasswordValidator
{
  public interface IPasswordValidationSettings
  {
    int MinimumPasswordLength { get; set; }
    bool NeedsNumber { get; set; }
    bool NeedsLetter { get; set; }
  }
}