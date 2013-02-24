﻿#region Copyright notice
/******************************************************************************
 * Copyright (C) 2013 Colin Angus Mackay
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 * 
 ******************************************************************************
 *
 * For more information visit: 
 * https://github.com/colinangusmackay/Xander.PasswordValidator
 * 
 *****************************************************************************/
#endregion

using System;
using System.Linq;

namespace Xander.PasswordValidator.Handlers
{
  public class ValidationServiceLocator
  {
    private readonly IPasswordValidationSettings _settings;
    public ValidationServiceLocator(IPasswordValidationSettings settings)
    {
      _settings = settings;
    }

    public static ValidationHandler GetValidationHandler(IPasswordValidationSettings settings)
    {
      var locator = new ValidationServiceLocator(settings);
      var result = locator.GetValidationHandler();
      return result;
    }

    public ValidationHandler GetValidationHandler()
    {
      ValidationHandler result = GetMinimumLengthHandler();
      ValidationHandler tail = GetNeedsNumberHandler(result);
      tail = GetNeedsLetterHandler(tail);
      tail = GetNeedsSymbolHandler(tail);
      tail = GetStandardWordListHandler(tail);
      tail = GetCustomWordListHandler(tail);
      tail = GetCustomHandlers(tail);
      return result;
    }

    private ValidationHandler GetNeedsSymbolHandler(ValidationHandler tail)
    {
      if (!_settings.NeedsSymbol)
        return tail;

      var newTail = new NeedsSymbolValidationHandler(_settings);
      tail.Successor = newTail;
      return newTail;
    }

    private ValidationHandler GetCustomHandlers(ValidationHandler tail)
    {
      ValidationHandler newTail = tail;
      foreach (Type handlerType in _settings.CustomValidators)
      {
        newTail = (ValidationHandler) Activator.CreateInstance(handlerType, _settings);
        tail.Successor = newTail;
        tail = newTail;
      }
      return newTail;
    }

    private ValidationHandler GetCustomWordListHandler(ValidationHandler tail)
    {
      if (!_settings.CustomWordLists.Any())
        return tail;

      var newTail = new CustomWordListValidationHandler(_settings);
      tail.Successor = newTail;
      return newTail;
    }

    private ValidationHandler GetStandardWordListHandler(ValidationHandler tail)
    {
      if (!_settings.StandardWordLists.Any())
        return tail;

      var newTail = new StandardWordListValidationHandler(_settings);
      tail.Successor = newTail;
      return newTail;
    }

    private ValidationHandler GetNeedsLetterHandler(ValidationHandler tail)
    {
      if (!_settings.NeedsLetter)
        return tail;

      var newTail = new NeedsLetterValidationHandler(_settings);
      tail.Successor = newTail;
      return newTail;
    }

    private ValidationHandler GetNeedsNumberHandler(ValidationHandler tail)
    {
      if (!_settings.NeedsNumber)
        return tail;

      var newTail = new NeedsNumberValidationHandler(_settings);
      tail.Successor = newTail;
      return newTail;
    }

    private MinimumLengthValidationHandler GetMinimumLengthHandler()
    {
      return new MinimumLengthValidationHandler(_settings);
    }
  }
}