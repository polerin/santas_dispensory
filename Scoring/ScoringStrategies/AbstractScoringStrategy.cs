using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SMG.Santas.Scoring;
using SMG.Santas.ObjectScripts;

namespace SMG.Santas.Scoring {
  public abstract class AbstractScoringStrategy : IScoringStrategy {

    public string Slug() {
      return this.GetType().Name;
    }

    public PresentList ScoreBin(Collector SubjectBin) {
			return ScoreList(SubjectBin.GetPresentList(), SubjectBin.GetContentCount());
    }

    public abstract PresentList ScoreList(PresentList binList, Dictionary<string, int> binContents);
  }
}
