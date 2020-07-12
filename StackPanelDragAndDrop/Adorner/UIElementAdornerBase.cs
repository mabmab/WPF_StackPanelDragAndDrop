using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace StackPanelDragAndDrop
{
    ///++++++++++++++++++++++++++++++++++++++++++++++++++++
    /// <summary>
    /// UIElement�C����{�N���X
    /// </summary>
    ///++++++++++++++++++++++++++++++++++++++++++++++++++++
    public class UIElementAdornerBase : Adorner
    {
        //=================================================
        // �v���e�N�gCLR�v���p�e�B
        //=================================================

        #region �v���e�N�gCLR�v���p�e�B
        /// <summary>�����r�W���A���I�u�W�F�N�g</summary>
        protected Grid AdornerVisual { get; private set; } = new Grid();

        /// <summary>�����`��T�[�t�F�[�X</summary>
        protected AdornerLayer AdornerLayer { get; private set; } = null;
        #endregion

        //=================================================
        // �R���X�g���N�^�[
        //=================================================

        #region �R���X�g���N�^�[
        /// <summary>�R���X�g���N�^�[</summary>
        /// <param name="adornedElement">�C���G�������g</param>
        protected UIElementAdornerBase(UIElement adornedElement) : base(adornedElement)
        {
            // �����`��T�[�t�F�[�X�̏�����
            AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            AdornerLayer?.Add(this);
        }
        #endregion

        //=================================================
        // �p�u���b�N���\�b�h
        //=================================================

        #region �p�u���b�N���\�b�h
        /// <summary>�f�^�b�`</summary>
        public void Detach()
        {
            // �����`��T�[�t�F�[�X���炱�̑����I�u�W�F�N�g���폜
            AdornerLayer?.Remove(this);
        }
        #endregion

        //=================================================
        // �C����{�N���X�̃I�[�o�[���C�h
        //=================================================

        #region �C����{�N���X�̃I�[�o�[���C�h
        /// <summary>�r�W���A���q�v�f��</summary>
        /// <remarks>���1��Ԃ�</remarks>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>�r�W���A���q�v�f�̎擾</summary>
        /// <param name="index">�C���f�b�N�X</param>
        /// <returns>�r�W���A���v�f</returns>
        /// <exception cref="IndexOutOfRangeException">�C���f�b�N�X�̎w�肪�Ԉ���Ă���</exception>
        protected override Visual GetVisualChild(int index)
        {
            if (VisualChildrenCount <= index)
            {
                throw new IndexOutOfRangeException();
            }
            return AdornerVisual;
        }

        /// <summary>�q�v�f�ɕK�v�ȃ��C�A�E�g�̃T�C�Y�𑪒�</summary>
        /// <param name="availableSize">�e�v�f���q�v�f�����蓖�Ă邱�Ƃ��ł���g�p�\�ȗ̈�</param>
        /// <returns>�q�v�f�ɕK�v�ȃ��C�A�E�g�̃T�C�Y</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // �����r�W���A���I�u�W�F�N�g�̃��C�A�E�g�X�V
            AdornerVisual.Measure(availableSize);
            // �q�v�f�ɕK�v�ȃ��C�A�E�g�̃T�C�Y�𑪒肵�Č��肵���T�C�Y��Ԃ�
            return base.MeasureOverride(availableSize);
        }

        /// <summary>�q�v�f��z�u</summary>
        /// <param name="finalSize">�v�f���̂Ǝq��z�u���邽�߂Ɏg�p����e�̖����̗̈�</param>
        /// <returns>�q�v�f��z�u���Č��肵���T�C�Y</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // �����r�W���A���I�u�W�F�N�g�̎q�v�f��z�u
            AdornerVisual.Arrange(new Rect(finalSize));
            // �q�v�f��z�u���Č��肵���T�C�Y��Ԃ�
            return base.ArrangeOverride(finalSize);
        }
        #endregion
    }
}
